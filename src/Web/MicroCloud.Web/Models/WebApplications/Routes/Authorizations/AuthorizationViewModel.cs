using System.ComponentModel.DataAnnotations;
using MicroCloud.Web.Models.WebApplications.Authentications;
using MicroCloud.Web.Models.WebApplications.Authorizations;
using WebApplication.Domain.WebApplication.Shared;

namespace MicroCloud.Web.Models.WebApplications.Routes;

public class AuthorizationViewModel
{
    [Required]
    public AuthorizationType Type { get; set; }
    public ICollection<AuthenticationSchemeViewModel> AuthenticationSchemes { get; set; }
    public IList<int> PolicyIds { get; set; }
    public IList<PolicySaveModel> Policies { get; set; }
    public IList<ClaimViewModel> Claims { get; set; }
}
