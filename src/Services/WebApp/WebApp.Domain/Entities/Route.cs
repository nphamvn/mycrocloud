﻿using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Authorization;
using WebApp.Domain.Shared;
using WebApp.Routes;

namespace WebApp.Domain.Entities;
public class Route : BaseEntity
{
    public int RouteId { get; set; }
    public int AppId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MatchOrder { get; set; }
    public string MatchPath { get; set; }
    public List<HttpMethod> MatchMethods { get; set; }
    public RouteAuthorizationType AuthorizationType { get; set; }
    public RouteAuthorization Authorization { get; set; }
    public RouteValidation Validation { get; set; }
    public RouteResponseProvider ResponseProvider { get; set; }
    public RouteResponse Response { get; set; }
}
public class MatchMethodCollection : List<string>
{

}
public class RouteAuthorization
{
    public List<int> PolicyIds { get; set; }
    public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public List<int> AuthenticationSchemeIds { get; set; }
    public Dictionary<string, object> Claims { get; set; }
}
public class RouteValidation
{
    public ICollection<QueryParameterValidationItem> QueryParameters { get; set; }
    public ICollection<HeaderValidationItem> Headers { get; set; }
    public ICollection<BodyValidationItem> Body { get; set; }
}
public class QueryParameterValidationItem
{
    public string Name { get; set; }
    public ICollection<RouteValidationRule> Rules { get; set; }
}
public class HeaderValidationItem
{
    public string Name { get; set; }
    public ICollection<RouteValidationRule> Rules { get; set; }
}
public class BodyValidationItem
{
    public string Field { get; set; }
    public ICollection<RouteValidationRule> Rules { get; set; }
}
public class RouteResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RouteResponseProvider Provider { get; set; }
    public RouteMockResponse Mock { get; set; }
    public RouteResponseProxiedServer ResponseProxiedServer { get; set; }
    public RouteResponseTriggerFunction Function { get; set; }
}
public class RouteMockResponse
{
    public GeneratedValue StatusCode { get; set; }
    public List<HeaderItem> Headers { get; set; } = new();
    public GeneratedValue Body { get; set; }
}
public class HeaderItem
{
    public string Name { get; set; }
    public GeneratedValue Value { get; set; }
}
public class RouteResponseProxiedServer
{
}
public class RouteResponseTriggerFunction
{
    public int FunctionId { get; set; }
}

public abstract class RouteValidationRule
{
    public abstract string Name { get; }
}

public class RouteValidationRuleJsonConverter : JsonConverter<RouteValidationRule>
{
    public override RouteValidationRule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var readerClone = reader;
        readerClone.Read();
        if (readerClone.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }
        var propertyName = readerClone.GetString();
        if (propertyName != "name")
        {
            throw new JsonException();
        }
        readerClone.Read();
        if (readerClone.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }
        var name = readerClone.GetString();
        return name switch
        {
            "required" => JsonSerializer.Deserialize<RequiredRouteValidationRule>(ref reader, options),
            "minlength" => JsonSerializer.Deserialize<MinLengthRouteValidationRule>(ref reader, options),
            "between" => JsonSerializer.Deserialize<BetweenRouteValidationRule>(ref reader, options),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, RouteValidationRule value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case RequiredRouteValidationRule requiredRule:
                JsonSerializer.Serialize(writer, requiredRule, options);
                break;
            case MinLengthRouteValidationRule minLengthRule:
                JsonSerializer.Serialize(writer, minLengthRule, options);
                break;
        }
    }
}

public class RequiredRouteValidationRule : RouteValidationRule
{
    public override string Name => "required";
}
public class MinLengthRouteValidationRule : RouteValidationRule
{
    public override string Name => "minlength";
    public int Length { get; set; }
}
public class BetweenRouteValidationRule : RouteValidationRule
{
    public override string Name => "between";
    public int Min { get; set; }
    public int Max { get; set; }
}