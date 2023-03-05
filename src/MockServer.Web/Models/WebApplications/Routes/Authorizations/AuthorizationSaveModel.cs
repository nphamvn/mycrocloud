using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Routes.Authorizations;

public class AuthorizationSaveModel
{
    public AuthorizationType Type { get; set; }
    public IEnumerable<SelectListItem>? AuthorizationTypeSelectListItem { get; set; }
    public IList<int> PolicyIds { get; set; }
    public IEnumerable<SelectListItem>? PolicySelectListItem { get; set; }
    public List<int> AuthenticationSchemeIds { get; set; }
    public IList<AuthorizationPolicySaveModel> Policies { get; set; }
    public IEnumerable<SelectListItem>? AuthenticationSchemeSelectListItems { get; set; }
}
