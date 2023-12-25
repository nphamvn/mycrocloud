using System.Security.Claims;

namespace WebApp.Api.Rest;

public static class ClaimsPrincipalExtentions
{
    public static string GetUserId(this ClaimsPrincipal principal) {
        return principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
