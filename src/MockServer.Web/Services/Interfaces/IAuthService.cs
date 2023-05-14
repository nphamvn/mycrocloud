using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace MockServer.Web.Services;
public interface IAuthService {
    AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
    ClaimsPrincipal CreateUserPrincipal(IdentityUser user);
}