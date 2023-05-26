namespace MockServer.Core.WebApplications
{
    public class MockResponse : RouteIntegration
    {
        public int ResponseId { get; set; }
        public int RouteId { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string BodyText { get; set; }
        public string BodyTextFormat { get; set; }
        public int BodyTextRenderEngine { get; set; }
        public ResponseDelayType DelayType { get; set; }
        public int? DelayFixedTime { get; set; }
    }
    public enum ResponseDelayType
    {
        NoDelay = 0,
        Fixed
    }
}
