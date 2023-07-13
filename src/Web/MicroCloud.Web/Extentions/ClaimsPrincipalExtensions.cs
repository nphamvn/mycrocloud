using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace MicroCloud.Web.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static IdentityUser ToIdentityUser(this ClaimsPrincipal principal)
        {
            var user = new IdentityUser
            {
                Id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                UserName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            };
            return user;
        }
    }
}