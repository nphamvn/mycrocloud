using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using MockServer.Web.Models.WebApplications.Routes.Integrations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteViewModel
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Order { get; set; } = 1;
    public ResponseProvider ResponseProvider { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public List<string> Methods { get; set; }
    public AuthorizationViewModel Authorization { get; set; }
    public RouteIntegrationViewModel Integration { get; set; }
    public WebApplication WebApplication { get; set; }
    public IList<ValidationItem> QueryParamsValidation { get; set; }
    public IList<ValidationItem> HeadersValidation { get; set; }
    public IList<ValidationItem> BodyValidation { get; set; }
}

public class ValidationItem
{
    public string Name { get; set; }
    public IList<object> Rules { get; set; }
}