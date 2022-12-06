using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockServer.Core.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
    }
}