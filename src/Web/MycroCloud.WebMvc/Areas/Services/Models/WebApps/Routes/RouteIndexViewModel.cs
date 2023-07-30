using Microsoft.AspNetCore.Mvc.Rendering;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class RouteIndexViewModel
{
    public IEnumerable<RouteIndexItem> Routes { get; set; }
    public IEnumerable<SelectListItem> HttpMethodSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationTypeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationPolicySelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationAuthenticationSchemeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> ResponseProviderSelectListItem { get; set; }
}
public class RouteIndexItem
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MatchPath { get; set; }
    public List<string> MatchMethods { get; set; }
    public int MatchOrder { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public RouteAuthorizationType AuthorizationType { get; set; }
    public RouteResponseProvider ResponseProvider { get; set; }
}