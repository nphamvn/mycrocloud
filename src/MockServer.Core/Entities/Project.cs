using System;
namespace MockServer.Core.Entities
{
    public class Project : BaseEntity
    {
        public string Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool PrivateAccess { get; set; }
        public string PrivateKey { get; set; }
        public AppUser User { get; set; }
        public ICollection<Request> Requests { get; set; }
    }
}

