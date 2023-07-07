using System.Text.Json.Serialization;
using MockServer.Domain.WebApplication.Shared;

namespace MockServer.Domain.WebApplication.Entities;

public class AuthorizationEntity
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public List<int> PolicyIds { get; set; }
    public IEnumerable<AuthenticationSchemeEntity> AuthenticationSchemes { get; set; }
    public List<int> AuthenticationSchemeIds { get; set; }
    public Dictionary<string, object> Claims { get; set; }
}
