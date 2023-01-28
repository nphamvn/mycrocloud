using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Web.Models.Request;

public class AuthorizationConfigViewModel
{
    public AuthorizationType? Type { get; set; }
    public IList<int> AuthenticationSchemes { get; set; }
    public IList<Requirement> Requirements { get; set; } = new List<Requirement>();
    public IEnumerable<AppAuthentication>? AuthenticationSchemeSelectList { get; set; }
}
