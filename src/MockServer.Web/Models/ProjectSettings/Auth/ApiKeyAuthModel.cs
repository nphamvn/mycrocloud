using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.WebMVC.Models.ProjectSettings.Auth;

public class ApiKeyAuthModel : AuthBaseModel
{
    public AuthType Type { get; set; } = AuthType.ApiKey;
    public ApiKeyAuthenticationOptions Options { get; set; } = new();
}
