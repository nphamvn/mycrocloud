using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Routes.Authorizations;

public class Authorization
{
    public AuthorizationType Type { get; set; }
    public IList<AuthorizationPolicy> Policies { get; set; }
}
