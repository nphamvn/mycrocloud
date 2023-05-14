using System.Text.Json.Serialization;

namespace MockServer.Web.Models.Common;

public class GitHubResponse{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
}