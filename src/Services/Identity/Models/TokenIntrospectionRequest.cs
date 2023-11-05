using System.Text.Json.Serialization;

namespace Identity.Models;

public class TokenIntrospectionRequest
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
    [JsonPropertyName("token_type_hint")]
    public string TokenTypeHint { get; set; }
}