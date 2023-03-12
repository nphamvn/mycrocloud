using MockServer.Core.WebApplications.Security;
using Policy = MockServer.Web.Models.WebApplications.Authorizations.Policy;
namespace MockServer.Web.Models.WebApplications.Routes.Authorizations;

public class Authorization
{
    public AuthorizationType Type { get; set; }
    public IList<Policy> Policies { get; set; }
}
