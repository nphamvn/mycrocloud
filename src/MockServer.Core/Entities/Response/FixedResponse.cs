using System;
namespace MockServer.Core.Entities
{
    public class FixedResponse : BaseEntity
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; }
        public string Body { get; set; }
        public int Delay { get; set; }
    }
}

