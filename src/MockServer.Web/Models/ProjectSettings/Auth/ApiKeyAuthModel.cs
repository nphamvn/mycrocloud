using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Web.Models.ProjectSettings.Auth;

public class ApiKeyAuthModel : AuthBaseModel
{
    public AuthenticationType Type { get; set; } = AuthenticationType.ApiKey;
    public ApiKeyAuthenticationOptions Options { get; set; } = new();
}
