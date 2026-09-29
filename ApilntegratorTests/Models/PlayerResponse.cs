using System.Text.Json.Serialization;

namespace ApilntegratorTests.Models;

public class PlayerResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("_id")]
    public string? UnderscoreId { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("surname")]
    public string? Surname { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonIgnore]
    public string EffectiveId => !string.IsNullOrWhiteSpace(Id) ? Id : UnderscoreId ?? string.Empty;
}
