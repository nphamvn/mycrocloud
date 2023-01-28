using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Web.Models.ProjectSettings.Auth;

public class JwtBearerAuthModel : AuthBaseModel
{
    public AuthenticationType Type { get; set; } = AuthenticationType.JwtBearer;
    public JwtBearerAuthenticationOptions Options { get; set; } = new();
}
