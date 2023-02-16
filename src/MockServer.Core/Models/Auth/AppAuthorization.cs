using System.Text.Json.Serialization;
using MockServer.Core.Enums;

namespace MockServer.Core.Models.Auth;

public class Authorization
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthorizationType Type { get; set; }
    public List<int> AuthenticationSchemes { get; set; }
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

public class Policy
{
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
    public string ConditionalExpression { get; set; }
    public bool Active { get; set; }
}
