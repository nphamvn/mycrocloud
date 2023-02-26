using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Authorizations;

public class AuthorizationViewModel
{
    public AuthorizationType Type { get; set; }
    public IList<PolicyViewModel> Policies { get; set; }
}
