using System.Text.Json.Serialization;

namespace MockServer.Core.WebApplications.Security;

public class Authorization
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public List<int> PolicyIds { get; set; }
    public List<int> AuthenticationSchemeIds { get; set; }
    public Dictionary<string, object> Claims { get; set; }
}
