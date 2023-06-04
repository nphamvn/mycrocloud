using System.Text.Json.Serialization;

namespace MockServer.Core.WebApplications.Security;

public class Authorization
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public IList<int> AuthenticationSchemeIds { get; set; }
    [JsonIgnore]
    public IList<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public IList<int> PolicyIds { get; set; }
    [JsonIgnore]
    public IList<Policy> Policies { get; set; }
    public static Authorization Authorize()
    {
        return new Authorization { Type = AuthorizationType.Authorized };
    }
    public static Authorization AllowAnonymous()
    {
        return new Authorization { Type = AuthorizationType.AllowAnonymous };
    }
}
