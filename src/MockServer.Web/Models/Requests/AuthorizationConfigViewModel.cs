using MockServer.Core.Models.Auth;
using MockServer.Core.Enums;

namespace MockServer.Web.Models.Requests;

public class AuthorizationConfigViewModel
{
    public AuthorizationType? Type { get; set; }
    public IList<int> AuthenticationSchemes { get; set; }
    public IList<Policy> Requirements { get; set; } = new List<Policy>();
    public IEnumerable<AuthenticationScheme>? AuthenticationSchemeSelectList { get; set; }
}
