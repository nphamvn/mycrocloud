using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.WebApplications;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteSaveModel
{
    [Required]
    public RouteIntegrationType IntegrationType { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
    public string Name { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Name length can't be more than 8.")]
    public string Path { get; set; }
    [Required]
    public string Method { get; set; }
    public string Description { get; set; }
    public IList<RequestQueryValidationItemSaveModel> RequestQueryValidationItems { get; set; }
    public IList<RequestHeaderValidationItemSaveModel> RequestHeaderValidationItems { get; set; }
    public IList<RequestBodyValidationItemSaveModel> RequestBodyValidationItems { get; set; }
    public WebApplication? WebApplication { get; set; }
    public IEnumerable<SelectListItem>? HttpMethodSelectListItems { get; set; }
}
