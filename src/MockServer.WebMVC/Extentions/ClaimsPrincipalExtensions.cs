using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MockServer.Core.Models;

namespace MockServer.WebMVC.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static T Parse<T>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            if (typeof(T) == typeof(ApplicationUser))
            {
                var user = new ApplicationUser
                {
                    Id = Convert.ToInt32(principal.FindFirstValue("sub")),
                    Username = principal.FindFirstValue("name"),
                    Email = principal.FindFirstValue("email")
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