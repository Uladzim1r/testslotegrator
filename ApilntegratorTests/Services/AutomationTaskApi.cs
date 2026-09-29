using ApilntegratorTests.Interfaces;
using ApilntegratorTests.Models;
using Refit;

namespace ApilntegratorTests.Services;

public sealed class AutomationTaskApi : IAutomationTaskApi
{
    private readonly IAutomationApi _api;

    public AutomationTaskApi(IAutomationApi api)
    {
        _api = api;
    }

    public Task<IApiResponse<PlayerResponse>> CreatePlayerAsync(PlayerCreateRequest request)
    {
        return _api.CreatePlayerAsync(request);
    }

    public Task<IApiResponse<PlayerResponse>> GetPlayerAsync()
    {
        return _api.GetPlayerAsync();
    }

    public Task<IApiResponse<PlayersListResponse>> GetAllPlayersAsync()
    {
        return _api.GetAllPlayersAsync();
    }

    public Task<IApiResponse<object>> DeletePlayerAsync(string id)
    {
        return _api.DeletePlayerAsync(id);
    }
}
