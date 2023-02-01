using MockServer.Core.Models.Auth;

namespace MockServer.Web.Models.ProjectSettings.Auth;

public class JwtBearerTokenGenerateModel
{
    public string Token { get; set; }
    public List<AppClaim> Claims { get; set; }
}
