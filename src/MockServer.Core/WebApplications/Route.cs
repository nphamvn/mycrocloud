using System.Text.Json;
using System.Text.Json.Serialization;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Core.WebApplications;
public class Route
{
    public int RouteId { get; set; }
    public int WebApplicationId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Match Match { get; set; }
    public Authorization Authorization { get; set; }
    public Validation Validation { get; set; }
    public Response Response { get; set; }
}

public class Response
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ResponseProvider Provider { get; set; }
    public MockResponse Mock { get; set; }
    public ProxiedServer ProxiedServer { get; set; }
    public Function Function { get; set; }
}
public class ProxiedServer
{
}
public class Function
{
    public int FunctionId { get; set; }
}

public class Match
{
    public int Order { get; set; }
    public ICollection<string> Methods { get; set; }
    public string Path { get; set; }
}

public class Validation
{
    public ICollection<QueryParameterValidationItem> QueryParameters { get; set; }
    public ICollection<HeaderValidationItem> Headers { get; set; }
    public ICollection<BodyValidationItem> Body { get; set; }
}

public class BodyValidationItem
{
    public string Field { get; set; }
    public ICollection<Rule> Rules { get; set; }
}

public class HeaderValidationItem
{
    public string Name { get; set; }
    public ICollection<Rule> Rules { get; set; }
}

public class QueryParameterValidationItem
{
    public string Name { get; set; }
    public ICollection<Rule> Rules { get; set; }
}
public class RuleValidateResult {
    public RuleValidateResult(bool isValid)
    {
        IsValid = isValid;
    }
    public RuleValidateResult(bool isValid, string theValueIsRequired)
    {
        IsValid = isValid;
        Message = theValueIsRequired;
    }
    public static RuleValidateResult Success()
    {
        return new RuleValidateResult(true);
    }
    
    public static RuleValidateResult Fail(string message)
    {
        return new RuleValidateResult(false, message);
    }
    public bool IsValid { get; private set; }
    public string Message { get; private set; }
}
public interface IRule {
    RuleValidateResult Validate(object value);
}
public abstract class Rule: IRule
{
    public abstract string Name { get; }
    public abstract RuleValidateResult Validate(object value);
}

public class RuleJsonConverter : JsonConverter<Rule>
{
    public override Rule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            "required" => JsonSerializer.Deserialize<RequiredRule>(ref reader, options),
            "minlength" => JsonSerializer.Deserialize<MinLengthRule>(ref reader, options),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, Rule value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case RequiredRule requiredRule:
                JsonSerializer.Serialize(writer, requiredRule, options);
                break;
            case MinLengthRule minLengthRule:
                JsonSerializer.Serialize(writer, minLengthRule, options);
                break;
        }
    }
}

public class RequiredRule : Rule
{
    public override string Name => "required";
    public override RuleValidateResult Validate(object value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return RuleValidateResult.Fail("The value is required.");
        }
        return RuleValidateResult.Success();
    }
}

public class MinLengthRule : Rule
{
    public override string Name => "minlength";
    public int Length { get; set; }
    public override RuleValidateResult Validate(object value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return RuleValidateResult.Fail("The value is required.");
        }

        var strValue = value.ToString();
        return strValue != null && strValue.Length < Length ? RuleValidateResult.Fail($"The value must have a minimum length of {Length}.") : RuleValidateResult.Success();
    }
}