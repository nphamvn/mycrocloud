using System.Text.Json.Serialization;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class RouteModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? MatchOrder { get; set; }
    public string MatchPath { get; set; }
    public List<string> MatchMethods { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RouteAuthorizationType AuthorizationType { get; set; }
    public RouteAuthorization Authorization { get; set; }
    public RouteValidation Validation { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RouteResponseProvider ResponseProvider { get; set; }
    public RouteResponse Response { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public abstract class RouteResponse
{

}
public class RouteMockResponse : RouteResponse
{
    public MockResponseStatusCode StatusCode { get; set; }
    public List<MockResponseHeader> Headers { get; set; }
}

public class MockResponseHeader
{
    public string Name { get; set; }
    public HttpRequestBasedValueProvider ValueProvider { get; set; }
    [JsonPropertyName("value")]
    public string StringValue { get; set; }
}

public class MockResponseStatusCode
{
    public HttpRequestBasedValueProvider ValueProvider { get; set; }
    [JsonPropertyName("value")]
    public string StringValue { get; set; }
}

public enum HttpRequestBasedValueProvider {
    Static,
    Evaluated
}

public class RouteValidation
{
    public List<RouteValidationQueryRule> QueryParamerters { get; set; }
    public List<RouteValidationHeaderRule> Headers { get; set; }
    public List<RouteValidationBodyFieldRulel> Body { get; set; }
}

public class RouteValidationBodyFieldRulel
{
}

public class RouteValidationHeaderRule
{
}

public class RouteValidationQueryRule
{
}

public class RouteAuthorization
{
    public RouteAuthorizationType Type { get; set; }
    public List<RouteAuthorizationPolicy> Policies { get; set; }
    public List<RouteAuthorizationClaim> Claims { get; set; }
}

public class RouteAuthorizationClaim
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class RouteAuthorizationPolicy
{
    public int PolicyId { get; set; }
    public string PolicyName { get; set; }
}