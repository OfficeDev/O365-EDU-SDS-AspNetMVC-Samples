/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneRosterProviderDemo.Exceptions;
using OneRosterProviderDemo.Vocabulary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OneRosterProviderDemo.Models
{
    public abstract class BaseModel
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public StatusType Status { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
        public string Metadata { get; set; }

        internal string Url(string baseUrl)
        {
            return $"{baseUrl}/{UrlType()}/{Id}";
        }
        internal abstract string ModelType();
        internal abstract string UrlType();

        public BaseModel()
        {
            DateTime time = DateTime.Now;
            CreatedAt = time;
            UpdatedAt = time;
        }

        public void AsJsonReference(JsonWriter jw, string baseUrl)
        {
            jw.WriteStartObject();

            jw.WritePropertyName("href");
            jw.WriteValue(Url(baseUrl));
            jw.WritePropertyName("sourcedId");
            jw.WriteValue(Id);
            jw.WritePropertyName("type");
            jw.WriteValue(ModelType());

            jw.WriteEndObject();
        }

        public void AsJson(JsonWriter jw, string baseUrl)
        {
            jw.WritePropertyName("sourcedId");
            jw.WriteValue(Id);
            jw.WritePropertyName("status");
            jw.WriteValue(Enum.GetName(typeof(Vocabulary.StatusType), Status));
            jw.WritePropertyName("dateLastModified");
            jw.WriteValue(UpdatedAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffz"));

            if (!String.IsNullOrEmpty(Metadata))
            {
                jw.WritePropertyName("metadata");
                jw.WriteValue(Metadata);
            }
        }

        public static IQueryable<T> ApplySort<T>(IQueryable<T> modelQuery, StringValues dataFields, StringValues orderBy)
        {
            if (dataFields.Count < 1)
            {
                return modelQuery;
            }

            var dataField = dataFields.First();
            if (orderBy.Count < 1 || orderBy.First().ToLower() == "asc")
            {
                return modelQuery.OrderBy(TranslateSort<T, Object>(dataField)).AsQueryable();
            }

            return modelQuery.OrderByDescending(TranslateSort<T, Object>(dataField)).AsQueryable();
        }

        public static Func<T, Object> TranslateSort<T, Object>(string fieldName)
        {
            try
            {
                Type modelType = typeof(T);

                var lookupMethod = modelType.GetMethod("LookupProperty", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                var modelProp = (PropertyInfo)lookupMethod.Invoke(null, new object[] { modelType, fieldName });

                var getter = modelProp.GetMethod;

                return (u => (Object)getter.Invoke(u, null));
            }
            catch (Exception)
            {
                throw new InvalidSortFieldException(fieldName);
            }
        }

        public static IQueryable<T> ApplyFilter<T>(IQueryable<T> modelQuery, StringValues filterValues)
        {
            Type modelType = typeof(T);

            var query = modelQuery;

            if (filterValues.Count > 0)
            {
                var filter = filterValues.First();

                string logicalOperator = null;
                string left = null;
                string right = null;

                if (filter.Contains(" AND "))
                {
                    logicalOperator = "AND";
                    var pieces = filter.Split(" AND ");
                    left = pieces[0];
                    right = pieces[1];
                }
                else if (filter.Contains(" OR "))
                {
                    logicalOperator = "OR";
                    var pieces = filter.Split(" OR ");
                    left = pieces[0];
                    right = pieces[1];
                }
                else
                {
                    left = filter;
                }

                if (logicalOperator == null)
                {
                    return query.Where(TranslateFilter<T>(left)).AsQueryable<T>();
                }

                var leftQuery = query.Where(TranslateFilter<T>(left)).AsQueryable<T>();
                var rightQuery = query.Where(TranslateFilter<T>(right)).AsQueryable<T>();

                if (logicalOperator == "AND")
                {
                    return leftQuery.Intersect(rightQuery);
                }
                else
                {
                    return leftQuery.Union(rightQuery);
                }
            }
            return query;
        }

        private static bool StringEquals(string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        private static bool StringNotEquals(string a, string b)
        {
            return !StringEquals(a, b);
        }

        private static bool StringGreaterThan(string a, string b)
        {
            return String.Compare(a, b, true) > 0;
        }

        private static bool StringLessThan(string a, string b)
        {
            return String.Compare(a, b, true) < 0;
        }

        private static bool StringGTE(string a, string b)
        {
            return StringGreaterThan(a, b) || StringEquals(a, b);
        }

        private static bool StringLTE(string a, string b)
        {
            return StringLessThan(a, b) || StringEquals(a, b);
        }

        private static bool StringContains(string a, string b)
        {
            return a.ToLower().Contains(b);
        }

        private static Dictionary<string, Func<string, string, bool>> StringComparatorMap = new Dictionary<string, Func<string, string, bool>>()
        {
            ["="] = StringEquals,
            ["!="] = StringNotEquals,
            [">"] = StringGreaterThan,
            [">="] = StringGTE,
            ["<"] = StringLessThan,
            ["<="] = StringLTE,
            ["~"] = StringContains
        };

        // For numbers, dates/times
        // https://stackoverflow.com/a/11113450
        private static Dictionary<string, string> ComparatorMap = new Dictionary<string, string>()
        {
            ["="] = "op_Equality",
            ["!="] = "op_Inequality",
            [">"] = "op_GreaterThan",
            [">="] = "op_GreaterThanOrEqual",
            ["<"] = "op_LessThan",
            ["<="] = "op_LessThanOrEqual",
            ["~"] = "Contains"
        };

        // https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997
        private static Regex filterMatcher = new Regex("(.*)(=|!=|>|>=|<|<=|~)'(.*)'");

        public static Func<T, bool> TranslateFilter<T>(string filter)
        {

            Type modelType = typeof(T);
            var match = filterMatcher.Match(filter);

            if (!match.Success)
            {
                throw new InvalidFilterFieldException(filter);
            }

            var dataFieldRaw = match.Groups[1].Value;
            var dataField = dataFieldRaw.ToLower();
            var predicate = match.Groups[2].Value;
            var value = match.Groups[3].Value.ToLower();

            var lookupMethod = modelType.GetMethod("LookupProperty", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            try
            {
                var modelProp = (PropertyInfo)lookupMethod.Invoke(null, new object[] { modelType, dataField });

                var fieldType = modelProp.PropertyType;
                var getter = modelProp.GetMethod;
                var comparisonValue = fieldType.IsEnum ? value : Convert.ChangeType(value, fieldType);

                var comparator = fieldType.IsEnum ? typeof(String).GetMethod(ComparatorMap[predicate]) : fieldType.GetMethod(ComparatorMap[predicate]);

                if (fieldType == typeof(String))
                {
                    var stringComparator = StringComparatorMap[predicate];

                    return (u => stringComparator.Invoke((string)getter.Invoke(u, null), (string)comparisonValue));
                }
                else if (!fieldType.IsEnum)
                {
                    if (comparator.IsStatic)
                    {
                        return (u => (bool)comparator.Invoke(null, new object[] { getter.Invoke(u, null), comparisonValue }));
                    }
                    return (u => (bool)comparator.Invoke(getter.Invoke(u, null), new object[] { comparisonValue }));
                }
                else
                {
                    if (comparator.IsStatic)
                    {
                        return (u => (bool)comparator.Invoke(null, new object[] { Enum.GetName(fieldType, getter.Invoke(u, null)).ToLower(), comparisonValue }));
                    }
                    return (u => (bool)comparator.Invoke(Enum.GetName(fieldType, getter.Invoke(u, null)).ToLower(), new object[] { comparisonValue }));
                }
            }
            catch (Exception)
            {
                throw new InvalidFilterFieldException(dataFieldRaw);
            }
        }

        private static Dictionary<string, string> PropertyMap = new Dictionary<string, string>()
        {
            ["datelastmodified"] = "UpdatedAt",
            ["sourcedid"] = "Id"
        };
        public static PropertyInfo LookupProperty(Type modelType, string propName)
        {
            var modelProp = modelType.GetProperties().FirstOrDefault(prop => prop.Name.ToLower() == propName.ToLower());

            if (modelProp == null)
            {
                modelProp = modelType.GetProperties().FirstOrDefault(prop => prop.Name == PropertyMap[propName.ToLower()]);
            }

            return modelProp;
        }

        internal string GetOptionalJsonProperty(JObject json, string property, string defaultValue)
        {
            if (json.TryGetValue(property, out JToken value))
            {
                return (string)value;
            }
            return defaultValue;
        }

        public static void CsvHeader(CsvWriter writer)
        {
            writer.WriteField("sourcedId");

            writer.WriteField("status");
            writer.WriteField("dateLastModified");
        }

        public void AsCsvRow(CsvWriter writer, bool bulk = true)
        {
            writer.WriteField(Id);

            if (bulk)
            {
                writer.WriteField("");
                writer.WriteField("");
            }
            else
            {
                writer.WriteField(Status);
                writer.WriteField(UpdatedAt.ToString("yyyy-MM-dd"));
            }
        }
    }
}
