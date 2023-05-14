using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace MockServer.Web.Services;
public class AuthService : IAuthService
{
    public const string LoginProviderKey = "LoginProvider";

    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        properties.Items[LoginProviderKey] = provider;
        return properties;
    }

    public ClaimsPrincipal CreateUserPrincipal(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
            // Add any other required claims here...
        };
        var identity = new ClaimsIdentity(claims, "LOCAL AUTHORITY");
        return new ClaimsPrincipal(identity);
    }
}