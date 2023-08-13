using System.Security.Claims;
using MycroCloud.WebMvc.Identity;

namespace MycroCloud.WebMvc.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static MycroCloudIdentityUser ToMycroCloudUser(this ClaimsPrincipal principal)
        {
            var user = new MycroCloudIdentityUser
            {
                Id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                UserName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            };
            return user;
        }
    }
}