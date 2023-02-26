using MockServer.Core.WebApplications;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Api.TinyFramework;

public class Request
{
    public int Id { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public RouteIntegrationType Type { get; set; }
    public WebApplication WebApplication { get; set; }
    public HttpContext HttpContext { get; set; }
    public Authorization Authorization { get; set; }
    public IList<RouteRequestHeader> RequestHeaders { get; set; }
    public IList<RouteRequestQuery> RequestQueries { get; set; }
    public RouteRequestBody Body { get; set; }
    public RouteIntegration Integration { get; set; } 
}
