/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using Microsoft.AspNetCore.Builder;
using OneRosterProviderDemo.Middleware;

namespace OneRosterProviderDemo.Middleware
{
    public static class OAuthExtension
    {
        public static IApplicationBuilder UseOauthMessageSigning(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OAuth>();
        }
    }
}
