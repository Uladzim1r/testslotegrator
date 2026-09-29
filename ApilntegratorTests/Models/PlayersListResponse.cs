using System.Text.Json.Serialization;

namespace ApilntegratorTests.Models;

public class PlayersListResponse
{
    [JsonPropertyName("players")]
    public List<PlayerResponse>? Players { get; set; }

    [JsonPropertyName("data")]
    public List<PlayerResponse>? Data { get; set; }

    [JsonPropertyName("items")]
    public List<PlayerResponse>? Items { get; set; }

    [JsonIgnore]
    public List<PlayerResponse> EffectiveItems => Players ?? Data ?? Items ?? new List<PlayerResponse>();
}
