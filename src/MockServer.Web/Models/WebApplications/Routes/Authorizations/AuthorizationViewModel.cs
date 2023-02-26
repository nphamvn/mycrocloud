using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Routes.Authorizations;

public class AuthorizationViewModel
{
    public AuthorizationType Type { get; set; }
    public IList<AuthorizationPolicyViewModel> Policies { get; set; }
}
