using System.Security.Claims;
using MockServer.Core.Models.Auth;

namespace MockServer.Api.TinyFramework;

public interface IAuthorizationService
{
    bool CheckRequirement(Policy requirement, ClaimsPrincipal user);
}
