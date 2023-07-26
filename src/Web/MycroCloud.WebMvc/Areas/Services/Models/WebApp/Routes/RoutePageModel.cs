using Microsoft.AspNetCore.Mvc.Rendering;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApp;

public class RoutePageModel
{
    public WebAppModel WebAppModel { get; set; }
    public IEnumerable<RouteIndexItem> Routes { get; set; }
    public IEnumerable<SelectListItem> HttpMethodSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationTypeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationPolicySelectListItem { get; set; }
    public IEnumerable<SelectListItem> AuthorizationAuthenticationSchemeSelectListItem { get; set; }
    public IEnumerable<SelectListItem> ResponseProviderSelectListItem { get; set; }
}