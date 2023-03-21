using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Web.Shared;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RoutePageModel
{
    public WebApplication WebApplication { get; set; }
    public IEnumerable<RouteIndexItem> Routes { get; set; }
    public IEnumerable<SelectListItem> HttpMethodSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationTypeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> IntegrationTypeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> PolicySelectListItem { get; set; }
    public IEnumerable<BuiltInValdationAttributeDescription> BuiltInValdationAttributes { get; set; }
}