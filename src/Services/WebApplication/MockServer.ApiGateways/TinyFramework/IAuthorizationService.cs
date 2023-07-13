using System.Security.Claims;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Api.TinyFramework;

public interface IAuthorizationService
{
    bool CheckRequirement(Policy requirement, ClaimsPrincipal user);
}
