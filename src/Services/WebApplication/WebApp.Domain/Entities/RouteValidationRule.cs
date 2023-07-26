using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Domain.Entities;


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
public class BetweenRouteValidationRule: RouteValidationRule {
    public override string Name => "between";
    public int Min { get; set; }
    public int Max { get; set; }
}
