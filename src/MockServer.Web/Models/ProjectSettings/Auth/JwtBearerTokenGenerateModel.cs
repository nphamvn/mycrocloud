namespace MockServer.Web.Models.ProjectSettings.Auth;

public class JwtBearerTokenGenerateModel
{
    public string Token { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}
