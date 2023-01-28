using System.Text.Json.Serialization;
using MockServer.Core.Enums;

namespace MockServer.Core.Models.Auth;

public class AppAuthorization
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public List<int> AuthenticationSchemes { get; set; }
    public IList<Requirement> Requirements { get; set; }
    public static AppAuthorization Authorize()
    {
        return new AppAuthorization { Type = AuthorizationType.Authorize };
    }
    public static AppAuthorization AllowAnonymous()
    {
        return new AppAuthorization { Type = AuthorizationType.AllowAnonymous };
    }
}

public class Requirement
{
    public string Name { get; set; }
    public string ConditionalExpression { get; set; }
    public bool Active { get; set; }
}
