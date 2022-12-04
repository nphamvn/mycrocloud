using System;
namespace MockServer.Core.Entities
{
    public class FixedRequest : BaseEntity
    {
        public int RequestId { get; set; }
        public int ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseContentType { get; set; }
        public int Delay { get; set; }
    }

    public class FixedRequestHeader : BaseEntity
    {
        public int FixedRequestId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}

