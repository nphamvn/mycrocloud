using System.Security.Claims;
using MockServer.Core.Identity;

namespace MockServer.Web.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static T Parse<T>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            if (typeof(T) == typeof(User))
            {
                var user = new User
                {
                    Id = Convert.ToInt32(principal.FindFirstValue("sub")),
                    Username = principal.FindFirstValue("name"),
                    Email = principal.FindFirstValue("email"),
                    AvatarUrl = principal.FindFirstValue("avatar_url")
                };

                return (T)Convert.ChangeType(user, typeof(T));
            }
            else
            {
                throw new Exception("Invalid type provided");
            }
        }
    }
}