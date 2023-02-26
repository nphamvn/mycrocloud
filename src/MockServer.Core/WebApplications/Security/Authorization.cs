using System.Text.Json.Serialization;

namespace MockServer.Core.WebApplications.Security;

public class Authorization
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public IList<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public IList<Policy> Policies { get; set; }
    public static Authorization Authorize()
    {
        return new Authorization { Type = AuthorizationType.Authorize };
    }
    public static Authorization AllowAnonymous()
    {
        return new Authorization { Type = AuthorizationType.AllowAnonymous };
    }
}
