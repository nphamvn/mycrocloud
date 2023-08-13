// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Ocelot.Authentication.JwtBearer
{
    /// <summary>
    /// Default values used by bearer authentication.
    /// </summary>
    public static class JwtBearerDefaults
    {
        /// <summary>
        /// Default value for AuthenticationSchemeEntity property in the JwtBearerAuthenticationOptions
        /// </summary>
        public const string AuthenticationScheme = "Bearer";
    }
}
