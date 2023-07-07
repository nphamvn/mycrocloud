using System.Text.Json.Serialization;
using MockServer.Domain.WebApplication.Shared;

namespace MockServer.Domain.WebApplication.Route;

public class RouteResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RouteResponseProvider Provider { get; set; }
    public RouteMockResponse Mock { get; set; }
    public ProxiedServer ProxiedServer { get; set; }
    public Function Function { get; set; }
}
