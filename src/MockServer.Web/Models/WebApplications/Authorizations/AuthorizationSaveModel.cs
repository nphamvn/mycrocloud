using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Authorizations;

public class AuthorizationSaveModel
{
    [Required]
    public string Name { get; set; }
    public AuthorizationType Type { get; set; }
    public List<int> AuthenticationSchemeIds { get; set; }
    public IList<PolicySaveModel> Policies { get; set; }
    public IEnumerable<SelectListItem>? AuthenticationSchemeSelectListItems { get; set; }
}
