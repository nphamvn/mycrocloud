using System.Text.Json.Serialization;
using WebApplication.Domain.WebApplication.Shared;

namespace WebApplication.Domain.WebApplication.Entities;

public class Authorization
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public List<int> PolicyIds { get; set; }
    public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public List<int> AuthenticationSchemeIds { get; set; }
    public Dictionary<string, object> Claims { get; set; }
}
