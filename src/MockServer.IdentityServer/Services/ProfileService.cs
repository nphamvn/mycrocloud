using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Serilog;

namespace MockServer.IdentityServer.Services;

public class ProfileService : IProfileService
{
    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var requestedClaimTypes = context.RequestedClaimTypes;
        var user = context.Subject;

        // your implementation to retrieve the requested information
        //var claims = GetRequestedClaims(user, requestedClaimsTypes);
        context.IssuedClaims.Add(new Claim("email", "nam@npham.me"));
        context.IssuedClaims.Add(new Claim("name", "npham"));
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}
