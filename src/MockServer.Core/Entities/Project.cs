using System;
namespace MockServer.Core.Entities
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Private { get; set; }
        public string PrivateKey { get; set; }
    }
}

