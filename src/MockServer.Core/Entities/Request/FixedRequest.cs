using System;
namespace MockServer.Core.Entities
{
    public class FixedRequest : BaseEntity
    {
        public int RequestId { get; set; }
    }

    public class FixedRequestHeader : BaseEntity
    {
        public int FixedRequestId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}

