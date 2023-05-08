using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MockServer.Core.Identity;

namespace MockServer.Web.Services;
public interface IAuthService {
    Task<User> AuthenticateLocalUser(string email, string password);
    Task<User> AuthenticateExternalUser(ClaimsPrincipal externalUser);
    AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
    ClaimsPrincipal CreateUserPrincipal(IdentityUser user);
}