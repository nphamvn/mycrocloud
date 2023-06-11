using System.Text.Json.Serialization;

namespace MockServer.Core.WebApplications.Security;

public class Authorization
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public IList<int> PolicyIds { get; set; }
    public Dictionary<string, object> Claims { get; set; }
    public static Authorization Authorize()
    {
        return new Authorization { Type = AuthorizationType.Authorized };
    }
    public static Authorization AllowAnonymous()
    {
        return new Authorization { Type = AuthorizationType.AllowAnonymous };
    }
}
