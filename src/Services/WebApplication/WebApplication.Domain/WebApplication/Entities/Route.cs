using System.Text.Json.Serialization;
using WebApplication.Domain.WebApplication.Entities;
using WebApplication.Domain.WebApplication.Route;

namespace WebApplication.Domain.WebApplication.Entities
{
    public class Route
    {
        public int RouteId { get; set; }
        public int WebApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RouteMatch Match { get; set; }
        public Authorization Authorization { get; set; }
        public RouteValidation Validation { get; set; }
        public RouteResponse Response { get; set; }
        public WebApplication WebApplication { get; set; }
    }
    public class RouteMatch
    {
        public int Order { get; set; }
        public List<string> Methods { get; set; } = new ();
        public string Path { get; set; }
    }
    public class RouteValidation
    {
        public ICollection<QueryParameterValidationItem> QueryParameters { get; set; }
        public ICollection<HeaderValidationItem> Headers { get; set; }
        public ICollection<BodyValidationItem> Body { get; set; }
    }
    public class QueryParameterValidationItem
    {
        public string Name { get; set; }
        public ICollection<RouteValidationRule> Rules { get; set; }
    }
    public class HeaderValidationItem
    {
        public string Name { get; set; }
        public ICollection<RouteValidationRule> Rules { get; set; }
    }
    public class BodyValidationItem
    {
        public string Field { get; set; }
        public ICollection<RouteValidationRule> Rules { get; set; }
    }
    public class RouteResponse
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RouteResponseProvider Provider { get; set; }
        public RouteMockResponse Mock { get; set; }
        public ProxiedServer ProxiedServer { get; set; }
        public Function Function { get; set; }
    }
}