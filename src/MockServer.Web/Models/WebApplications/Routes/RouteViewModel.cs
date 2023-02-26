using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using MockServer.Web.Models.WebApplications.Routes.Integrations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RouteIntegrationType IntegrationType { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public AuthorizationViewModel Authorization { get; set; }
    public RouteIntegrationViewModel Integration { get; set; }
    public WebApplication WebApplication { get; set; }
    public IList<RouteRequestHeaderViewModel> RequestHeaders { get; set; }
    public IList<RouteRequestQueryViewModel> RequestQueries { get; set; }
    public RouteRequestBodyViewModel RequestBody { get; set; }
    public int ApplicationId { get; set; }
    public string Url => string.Format("https://{0}.{1}.mockserver.com:5000/{2}", WebApplication.Name, WebApplication.User.Username, Path);
    public IEnumerable<SelectListItem> StatusCodeSelectListItem { get; set; }
    public string MethodTextColor
        => Method switch
        {
            "DELETE" => "text-red",
            "POST" => "text-orange",
            "PUT" => "text-yellow",
            "GET" => "text-green",
            _ => "text-red"
        };
}
