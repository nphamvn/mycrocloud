using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Web.Models.ProjectSettings.Auth;

public class ApiKeyAuthModel : AuthenticationScheme
{
    public AuthenticationSchemeType Type { get; set; } = AuthenticationSchemeType.ApiKey;
    public ApiKeyAuthenticationOptions Options { get; set; } = new();
}
