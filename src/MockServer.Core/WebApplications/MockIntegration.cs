using System.Text.Json.Serialization;

namespace MockServer.Core.WebApplications
{
    public class MockResponse
    {
        public int ResponseId { get; set; }
        public int RouteId { get; set; }
        public Value StatusCode { get; set; }
        public ICollection<HeaderItem> Headers { get; set; }
        public Value Body { get; set; }
    }
    public class HeaderItem
    {
        public string Name { get; set; }
        public Value Value { get; set; }
    }

    public class Value
    {
        public TemplateRenderedValue RenderedValue { get; set; }
    }

    public class TemplateRenderedValue {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TemplateEngine Engine { get; set; }
        public string Template { get; set; }
        public string Format { get; set; }
    }
    public enum TemplateEngine {
        None,
        JavaScript, 
        Handlebars,
        Mustache,
        Ejs
    }
}