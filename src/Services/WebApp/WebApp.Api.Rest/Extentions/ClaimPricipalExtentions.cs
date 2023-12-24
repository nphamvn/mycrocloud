using System.Security.Claims;

namespace WebApp.Api.Rest;

public static class ClaimsPrincipalExtentions
{
    public static IdentityUser ToIdentityUser(this ClaimsPrincipal principal) {
        return new IdentityUser {
            UserId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
        };
    }
}
