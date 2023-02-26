using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications.Authorizations;
using MockServer.Web.Models.WebApplications.Routes.Integrations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class Route
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RouteIntegrationType IntegrationType { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public AuthorizationViewModel Authorization { get; set; }
    public IList<RouteRequestHeader> RequestHeaders { get; set; }
    public IList<RouteRequestQuery> RequestQueries { get; set; }
    public RouteRequestBody Body { get; set; }
    public RouteIntegrationViewModel Integration { get; set; }
    public int ApplicationId { get; set; }
    public WebApplication WebApplication { get; set; }
}
