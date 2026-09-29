namespace ApilntegratorTests.Generated;

public partial class LoginResponse
{
    public string EffectiveToken => !string.IsNullOrWhiteSpace(Token) ? Token : Access_token ?? string.Empty;
}

public partial class PlayerResponse
{
    public string EffectiveId => !string.IsNullOrWhiteSpace(Id) ? Id : _id ?? string.Empty;
}

public partial class PlayersListResponse
{
    public List<PlayerResponse> EffectiveItems =>
        (Players ?? Data ?? Items)?.ToList() ?? new List<PlayerResponse>();
}
