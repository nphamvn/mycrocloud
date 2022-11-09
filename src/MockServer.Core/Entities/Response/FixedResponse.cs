using System;
namespace MockServer.Core.Entities
{
	public class FixedResponse
	{
		public int Id { get; set; }
		public int ResponseId { get; set; }
		public int StatusCode { get; set; }
		public int Body { get; set; }
	}
}

