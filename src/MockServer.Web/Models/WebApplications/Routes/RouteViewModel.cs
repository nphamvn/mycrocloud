using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using MockServer.Web.Models.WebApplications.Routes.Integrations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteViewModel
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public ResponseProvider ResponseProvider { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public AuthorizationViewModel Authorization { get; set; }
    public RouteIntegrationViewModel Integration { get; set; }
    public WebApplication WebApplication { get; set; }
    public IList<RequestQueryValidationItemSaveModel> RequestQueryValidationItems { get; set; }
    public IList<RequestHeaderValidationItemSaveModel> RequestHeaderValidationItems { get; set; }
    public IList<RequestBodyValidationItemSaveModel> RequestBodyValidationItems { get; set; }
    public IEnumerable<SelectListItem> MethodSelectListItem { get; set; }
}
