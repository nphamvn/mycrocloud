using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Web.Shared;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteIndexViewModel
{
    public WebApplication WebApplication { get; set; }
    public IEnumerable<RouteIndexItem> Routes { get; set; }
    public IEnumerable<SelectListItem> HttpMethodSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationTypeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationPolicySelectListItem { get; set; }
    public IEnumerable<SelectListItem> ResponseProviderSelectListItem { get; set; }
    public IEnumerable<BuiltInValdationAttributeDescription> BuiltInValdationAttributes { get; set; }
}