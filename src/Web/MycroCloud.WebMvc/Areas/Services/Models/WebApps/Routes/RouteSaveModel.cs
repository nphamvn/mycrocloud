using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps
{
    public class RouteSaveModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? MatchOrder { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 50.")]
        public string MatchPath { get; set; }
        public List<string> MatchMethods { get; set; }
        public RouteAuthorizationSaveModel? Authorization { get; set; }
        public RouteValidationSaveModel? Validation { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RouteResponseProvider ResponseProvider { get; set; }
        [JsonConverter(typeof(RouteResponseSaveModelJsonConverter))]
        public RouteResponseSaveModel Response { get; set; }
    }
    public class RouteAuthorizationSaveModel
    {
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RouteAuthorizationType Type { get; set; }
        public List<int>? PolicyIds { get; set; }
        public List<RouteAuthorizationClaimSaveModel>? Claims { get; set; }
    }

    public class RouteValidationSaveModel
    {
        public List<RouteValidationQueryRuleSaveModel> QueryParameters { get; set; }
        public List<RouteValidationHeaderRuleSaveModel> Headers { get; set; }
        public List<RouteValidationBodyFieldRuleSaveModel> Body { get; set; }
    }

    public abstract class RouteResponseSaveModel
    {
        public const string ProviderDiscriminator = "$provider";
    }

    public class MockResponseSaveModel : RouteResponseSaveModel
    {

    }
    public class RouteResponseSaveModelJsonConverter : JsonConverter<RouteResponseSaveModel>
    {
        public override RouteResponseSaveModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerClone = reader;
            var provider = GetProviderType(readerClone);
            if (string.IsNullOrEmpty(provider))
            {
                throw new JsonException("'$provider' not found in the JSON object.");
            }
            return provider switch
            {
                nameof(RouteResponseProvider.Mock) => JsonSerializer.Deserialize<MockResponseSaveModel>(ref reader, options),
                _ => throw new JsonException()
            };

            static string GetProviderType(Utf8JsonReader reader)
            {
                while (reader.Read())
                {
                    if (reader.TokenType != JsonTokenType.PropertyName) continue;
                    var propertyName = reader.GetString();
                    if (propertyName != RouteResponseSaveModel.ProviderDiscriminator) continue;
                    if (!reader.Read() || reader.TokenType != JsonTokenType.String)
                        throw new JsonException("Invalid JSON format. Expected a string value for $provider.");
                    return reader.GetString();
                }
                return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, RouteResponseSaveModel response, JsonSerializerOptions options)
        {
            switch (response)
            {
                case MockResponseSaveModel mock:
                    JsonSerializer.Serialize(writer, mock, options);
                    break;
            }
        }
    }
    public class RouteValidationBodyFieldRuleSaveModel
    {
        public string Name { get; set; }
        public List<RouteRequestValidationRule> Rules { get; set; }
    }

    public class RouteValidationHeaderRuleSaveModel
    {
        public string Name { get; set; }
        public List<RouteRequestValidationRule> Rules { get; set; }
    }

    public class RouteValidationQueryRuleSaveModel
    {
        public string Name { get; set; }
        public List<RouteRequestValidationRule> Rules { get; set; }
    }

    public class RouteAuthorizationClaimSaveModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
    }

    [JsonConverter(typeof(RouteRequestValidationRuleJsonConverter))]
    public abstract class RouteRequestValidationRule
    {
        public abstract string Name { get; }
    }

    public class RouteRequestValidationRuleJsonConverter : JsonConverter<RouteRequestValidationRule>
    {
        public override RouteRequestValidationRule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                "required" => JsonSerializer.Deserialize<RequiredValidationRule>(ref reader, options),
                "minlength" => JsonSerializer.Deserialize<MinLengthValidationRule>(ref reader, options),
                "between" => JsonSerializer.Deserialize<BetweenValidationRule>(ref reader, options),
                _ => throw new JsonException()
            };
        }

        public override void Write(Utf8JsonWriter writer, RouteRequestValidationRule value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case RequiredValidationRule requiredRule:
                    JsonSerializer.Serialize(writer, requiredRule, options);
                    break;
                case MinLengthValidationRule minLengthRule:
                    JsonSerializer.Serialize(writer, minLengthRule, options);
                    break;
            }
        }
    }

    public class RequiredValidationRule : RouteRequestValidationRule
    {
        public override string Name => "required";
    }
    public class MinLengthValidationRule : RouteRequestValidationRule
    {
        public override string Name => "minlength";
        public int Length { get; set; }
    }
    public class BetweenValidationRule : RouteRequestValidationRule
    {
        public override string Name => "between";
        public int Min { get; set; }
        public int Max { get; set; }
    }
}