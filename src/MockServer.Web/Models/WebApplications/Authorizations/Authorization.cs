using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Authorizations;

public class Authorization
{
    public string Name { get; set; }
    public AuthorizationType Type { get; set; }
    public List<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public IList<Policy> Policies { get; set; }
}
