using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Web.Models.ProjectAuthentication.JwtBearer;

public class JwtBearerSchemeViewModel : AuthenticationScheme
{
    public override AuthenticationSchemeType Type { get; set; } = AuthenticationSchemeType.JwtBearer;
    public JwtBearerAuthenticationOptions Options { get; set; } = new();
}
