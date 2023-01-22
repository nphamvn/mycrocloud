using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.WebMVC.Models.ProjectSettings.Auth;

public class JwtBearerAuthModel : AuthBaseModel
{
    public AuthType Type { get; set; } = AuthType.JwtBearer;
    public JwtBearerAuthenticationOptions Options { get; set; } = new();
}
