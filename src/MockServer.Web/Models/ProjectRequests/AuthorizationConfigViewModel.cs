using MockServer.Core.Models.Auth;
using MockServer.Core.Enums;

namespace MockServer.Web.Models.ProjectRequests;

public class AuthorizationConfiguration
{
    public AuthorizationType? Type { get; set; }
    public IList<int> AuthenticationSchemes { get; set; }
    public IList<Policy> Policies { get; set; } = new List<Policy>();
    public IEnumerable<AuthenticationScheme>? AuthenticationSchemeSelectList { get; set; }
}
