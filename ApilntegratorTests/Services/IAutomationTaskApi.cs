using ApilntegratorTests.Generated;
using Refit;

namespace ApilntegratorTests.Services;

public interface IAutomationTaskApi
{
    Task<IApiResponse<PlayerResponse>> CreatePlayerAsync(PlayerCreateRequest request);

    Task<IApiResponse<PlayerResponse>> GetPlayerAsync();

    Task<IApiResponse<PlayersListResponse>> GetAllPlayersAsync();

    Task<IApiResponse<object>> DeletePlayerAsync(string id);
}
