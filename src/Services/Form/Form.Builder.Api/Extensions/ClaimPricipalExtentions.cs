using System.Security.Claims;

namespace Form.Builder.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal) {
        return principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
