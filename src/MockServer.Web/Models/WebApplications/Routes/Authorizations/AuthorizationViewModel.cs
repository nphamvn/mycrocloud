using System.ComponentModel.DataAnnotations;
using MockServer.Domain.WebApplication.Shared;
using MockServer.Web.Models.WebApplications.Authentications;
using MockServer.Web.Models.WebApplications.Authorizations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class AuthorizationViewModel
{
    [Required]
    public AuthorizationType Type { get; set; }
    public ICollection<AuthenticationSchemeViewModel> AuthenticationSchemes { get; set; }
    public IList<int> PolicyIds { get; set; }
    public IList<PolicySaveModel> Policies { get; set; }
    public IList<ClaimViewModel> Claims { get; set; }
}
