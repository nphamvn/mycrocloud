using Microsoft.AspNetCore.Mvc.Rendering;

namespace MicroCloud.Web.Models.WebApplications.Routes;

public class RouteIndexViewModel
{
    public WebApplication WebApplication { get; set; }
    public IEnumerable<RouteIndexItem> Routes { get; set; }
    public IEnumerable<SelectListItem> HttpMethodSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationTypeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationPolicySelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationAuthenticationSchemeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> ResponseProviderSelectListItem { get; set; }
}