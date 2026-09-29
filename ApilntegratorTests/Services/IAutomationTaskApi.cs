using ApilntegratorTests.Models;
using Refit;

namespace ApilntegratorTests.Services;

public interface IAutomationTaskApi
{
    Task<IApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<IApiResponse<PlayerResponse>> CreatePlayerAsync(PlayerCreateRequest request);
    Task<IApiResponse<PlayerResponse>> GetPlayerAsync();
    Task<IApiResponse<PlayersListResponse>> GetAllPlayersAsync();
    Task<IApiResponse<object>> DeletePlayerAsync(string id);
}
