using System.Text.Json.Serialization;
using WebApp.Domain.Shared;

namespace WebApp.Domain.Entities
{
    public class RouteEntity
    {
        public int RouteId { get; set; }
        public int WebAppId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RouteMatch Match { get; set; }
        public RouteAuthorization RouteAuthorization { get; set; }
        public RouteValidation Validation { get; set; }
        public RouteResponse Response { get; set; }
    }
    public class RouteAuthorization
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AuthorizationType Type { get; set; }
        public List<int> PolicyIds { get; set; }
        public IEnumerable<AuthenticationSchemeEntity> AuthenticationSchemes { get; set; }
        public List<int> AuthenticationSchemeIds { get; set; }
        public Dictionary<string, object> Claims { get; set; }
    }
    public class RouteMatch
    {
        public int Order { get; set; }
        public List<string> Methods { get; set; } = new();
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
        public RouteResponseProxiedServer ResponseProxiedServer { get; set; }
        public RouteResponseTriggerFunction Function { get; set; }
    }
    public class RouteMockResponse
    {
        public GeneratedValue StatusCode { get; set; }
        public List<HeaderItem> Headers { get; set; } = new ();
        public GeneratedValue Body { get; set; }
    }
    public class HeaderItem
    {
        public string Name { get; set; }
        public GeneratedValue Value { get; set; }
    }
    public class RouteResponseProxiedServer
    {
    }
    public class RouteResponseTriggerFunction
    {
        public int FunctionId { get; set; }
    }
}