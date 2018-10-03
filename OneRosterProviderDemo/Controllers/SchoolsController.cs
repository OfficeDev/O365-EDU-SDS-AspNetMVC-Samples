/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OneRosterProviderDemo.Models;
using Microsoft.EntityFrameworkCore;
using OneRosterProviderDemo.Serializers;
using System.Linq;

namespace OneRosterProviderDemo.Controllers
{
    [Route("ims/oneroster/v1p1/schools")]
    public class SchoolsController : BaseController
    {
        public SchoolsController(ApiContext _db) : base(_db)
        {

        }

        // GET ims/oneroster/v1p1/schools
        [HttpGet]
        public IActionResult GetAllSchools()
        {
            IQueryable<Models.Org> orgsQuery = db.Orgs
                .Where(o => o.Type == Vocabulary.OrgType.school)
                .Include(o => o.Parent)
                .Include(o => o.Children);
            orgsQuery = ApplyBinding(orgsQuery);
            var orgs = orgsQuery.ToList();

            serializer = new OneRosterSerializer("orgs");
            serializer.writer.WriteStartArray();
            foreach (var org in orgs)
            {
                org.AsJson(serializer.writer, BaseUrl());
            }
            serializer.writer.WriteEndArray();

            return JsonOk(FinishSerialization(), ResponseCount);
        }

        private Org LookupSchool(string id)
        {
            return db.Orgs
                .Where(o => o.Type == Vocabulary.OrgType.school)
                .Include(o => o.Parent)
                .Include(o => o.Children)
                .SingleOrDefault(o => o.Id == id);
        }

        // GET ims/oneroster/v1p1/schools/5
        [HttpGet("{id}")]
        public IActionResult GetSchool([FromRoute] string id)
        {
            var org = LookupSchool(id);

            if (org == null)
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("org");
            org.AsJson(serializer.writer, BaseUrl());
            return JsonOk(serializer.Finish());
        }

        // GET schools/{id}/courses
        [HttpGet("{id}/courses")]
        public IActionResult GetCoursesForSchool([FromRoute] string id)
        {
            var courses = db.Courses
                .Include(c => c.SchoolYearAcademicSession)
                .Include(c => c.Org)
                .Where(c => c.OrgId == id);

            if(!courses.Any())
            {
                return NotFound();
            }
            serializer = new OneRosterSerializer("courses");
            serializer.writer.WriteStartArray();
            foreach (var course in courses)
            {
                course.AsJson(serializer.writer, BaseUrl());
            }
            serializer.writer.WriteEndArray();

            return JsonOk(serializer.Finish());
        }

        // GET ims/oneroster/v1p1/schools/{id}/enrollments
        [HttpGet("{id}/enrollments")]
        public IActionResult GetEnrollmentsForSchool([FromRoute] string id)
        {
            var enrollments = db.Enrollments
                .Include(e => e.User)
                .Include(e => e.IMSClass)
                .Include(e => e.School)
                .Where(e => e.IMSClass.SchoolOrgId == id);

            enrollments = ApplyBinding(enrollments);

            if(!enrollments.Any())
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("enrollments");
            serializer.writer.WriteStartArray();
            foreach (var enrollment in enrollments)
            {
                enrollment.AsJson(serializer.writer, BaseUrl());
            }
            serializer.writer.WriteEndArray();

            return JsonOk(serializer.Finish(), ResponseCount);
        }

        // GET ims/oneroster/v1p1/schools/{id}/classes
        [HttpGet("{id}/classes")]
        public IActionResult GetClassesForSchool([FromRoute] string id)
        {
            var imsClasses = db.IMSClasses
                .Include(c => c.IMSClassAcademicSessions)
                    .ThenInclude(kas => kas.AcademicSession)
                .Include(c => c.Course)
                .Include(c => c.School)
                .Where(c => c.SchoolOrgId == id);

            imsClasses = ApplyBinding(imsClasses);

            if(!imsClasses.Any())
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("classes");
            serializer.writer.WriteStartArray();
            foreach (var imsClass in imsClasses)
            {
                imsClass.AsJson(serializer.writer, BaseUrl());
            }
            serializer.writer.WriteEndArray();
            return JsonOk(serializer.Finish(), ResponseCount);
        }   

        // GET ims/oneroster/v1p1/schools/{id}/students
        [HttpGet("{id}/students")]
        public IActionResult GetStudentsForSchool([FromRoute] string id)
        {
            var students = db.Users
                .Include(u => u.UserOrgs).ThenInclude(uo => uo.Org)
                .Include(u => u.UserAgents).ThenInclude(ua => ua.Agent)
                .Where(u => u.Role == Vocabulary.RoleType.student);

            students = ApplyBinding(students);

            if(!students.Any())
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("students");
            serializer.writer.WriteStartArray();
            foreach(var student in students)
            {
                foreach(var org in student.UserOrgs)
                {
                    if(org.OrgId == id)
                    {
                        student.AsJson(serializer.writer, BaseUrl());
                    }
                }
            }
            serializer.writer.WriteEndArray();
            return JsonOk(serializer.Finish(), ResponseCount);
        }

        // GET ims/oneroster/v1p1/schools/{id}/teachers
        [HttpGet("{id}/teachers")]
        public IActionResult GetTeachersForSchool([FromRoute] string id)
        {
            var teachers = db.Users
                .Include(u => u.UserOrgs).ThenInclude(uo => uo.Org)
                .Include(u => u.UserAgents).ThenInclude(ua => ua.Agent)
                .Where(u => u.Role == Vocabulary.RoleType.teacher);

            teachers = ApplyBinding(teachers);
    
            if(!teachers.Any())
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("teachers");
            serializer.writer.WriteStartArray();
            foreach (var teacher in teachers)
            {
                foreach(var org in teacher.UserOrgs)
                {
                    if(org.OrgId == id)
                    {
                        teacher.AsJson(serializer.writer, BaseUrl());
                    }
                }
            }
            serializer.writer.WriteEndArray();
            return JsonOk(serializer.Finish(), ResponseCount);
        }

        // GET ims/oneroster/v1p1/schools/{id}/terms
        [HttpGet("{id}/terms")]
        public IActionResult GetTermsForSchool([FromRoute] string id)
        {
            var courses = db.Courses
                .Include(c => c.SchoolYearAcademicSession)
                .Where(c => c.OrgId == id);

            if (!courses.Any())
            {
                return NotFound();
            }

            var terms = courses.Select(c => c.SchoolYearAcademicSession);

            serializer = new OneRosterSerializer("terms");
            serializer.writer.WriteStartArray();
            foreach (var term in terms)
            {
                term.AsJson(serializer.writer, BaseUrl());
            }
            serializer.writer.WriteEndArray();
            return JsonOk(serializer.Finish());
        }

        // GET ims/oneroster/v1p1/schools/{school_id}/classes/class_id}/enrollments
        [HttpGet("{schoolId}/classes/{classId}/enrollments")]
        public IActionResult GetEnrollmentsForClassInSchool([FromRoute] string schoolId, string classId)
        {
            var imsClass = db.IMSClasses
                .Where(c => c.SchoolOrgId == schoolId)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.IMSClass)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.School)
                .SingleOrDefault(c => c.Id == classId);

            if(imsClass == null)
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("enrollments");
            serializer.writer.WriteStartArray();
            foreach (var enrollment in imsClass.Enrollments)
            {
                enrollment.AsJson(serializer.writer, BaseUrl());
            }
            serializer.writer.WriteEndArray();
            return JsonOk(serializer.Finish());
        }

        // GET ims/oneroster/v1p1/schools/{school_id}/classes/{class_id}/students
        [HttpGet("{schoolId}/classes/{classId}/students")]
        public IActionResult GetStudentsForClassInSchool([FromRoute] string schoolId, string classId)
        {
            var imsClass = db.IMSClasses
                .Where(c => c.SchoolOrgId == schoolId)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                        .ThenInclude(u => u.UserOrgs)
                            .ThenInclude(uo => uo.Org)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                        .ThenInclude(u => u.UserAgents)
                            .ThenInclude(ua => ua.Agent)
                .SingleOrDefault(c => c.Id == classId);

            if(imsClass == null)
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("students");
            serializer.writer.WriteStartArray();
            foreach (var enrollment in imsClass.Enrollments)
            {
                var user = enrollment.User;
                if(user.Role == Vocabulary.RoleType.student)
                {
                    user.AsJson(serializer.writer, BaseUrl());
                }
            }
            serializer.writer.WriteEndArray();
            return JsonOk(serializer.Finish());
        }

        // GET ims/oneroster/v1p1/schools/{school_id}/classes/{class_id}/teachers
        [HttpGet("{schoolId}/classes/{classId}/teachers")]
        public IActionResult GetTeachersForClassInSchool([FromRoute] string schoolId, string classId)
        {
            var imsClass = db.IMSClasses
                .Where(c => c.SchoolOrgId == schoolId)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                        .ThenInclude(u => u.UserOrgs)
                            .ThenInclude(uo => uo.Org)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                        .ThenInclude(u => u.UserAgents)
                            .ThenInclude(ua => ua.Agent)
                .SingleOrDefault(c => c.Id == classId);

            if (imsClass == null)
            {
                return NotFound();
            }

            serializer = new OneRosterSerializer("teachers");
            serializer.writer.WriteStartArray();
            foreach (var enrollment in imsClass.Enrollments)
            {
                var user = enrollment.User;
                if (user.Role == Vocabulary.RoleType.teacher)
                {
                    user.AsJson(serializer.writer, BaseUrl());
                }
            }
            serializer.writer.WriteEndArray();
            return JsonOk(serializer.Finish());
        }
    }
}
