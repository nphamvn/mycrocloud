using Microsoft.AspNetCore.Mvc.Rendering;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class RouteIndexViewModel
{
    public WebAppModel WebApp { get; set; }
    public IEnumerable<RouteItem> Routes { get; set; }
    public IEnumerable<SelectListItem> HttpMethodSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationTypeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationPolicySelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationAuthenticationSchemeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> ResponseProviderSelectListItem { get; set; }
}
public class RouteItem
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public RouteMatchSaveModel Match { get; set; }
}