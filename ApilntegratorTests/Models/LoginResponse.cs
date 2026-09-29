using System.Text.Json.Serialization;

namespace ApilntegratorTests.Models;

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonIgnore]
    public string EffectiveToken => !string.IsNullOrWhiteSpace(Token) ? Token : AccessToken ?? string.Empty;
}
