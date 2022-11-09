using System;
namespace MockServer.Core.Entities.Request
{
	public class FixedRequest
	{
		public int RequestId { get; set; }
	}

	public class FixedRequestHeader
	{
		public int FixedRequestId { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public int Order { get; set; }
	}
}

