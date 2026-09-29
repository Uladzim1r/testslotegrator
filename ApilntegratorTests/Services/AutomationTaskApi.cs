using ApilntegratorTests.Interfaces;
using ApilntegratorTests.Models;
using Refit;

namespace ApilntegratorTests.Services;

public sealed class AutomationTaskApi : IAutomationTaskApi
{
    private readonly IAutomationApi _api;
    private readonly IAuthenticationService _authenticationService;

    public AutomationTaskApi(IAutomationApi api, IAuthenticationService authenticationService)
    {
        _api = api;
        _authenticationService = authenticationService;
    }

    public Task<IApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        return _api.LoginAsync(request);
    }

    public Task<IApiResponse<PlayerResponse>> CreatePlayerAsync(PlayerCreateRequest request)
    {
        return _api.CreatePlayerAsync(request, _authenticationService.Token);
    }

    public Task<IApiResponse<PlayerResponse>> GetPlayerAsync()
    {
        return _api.GetPlayerAsync(_authenticationService.Token);
    }

    public Task<IApiResponse<PlayersListResponse>> GetAllPlayersAsync()
    {
        return _api.GetAllPlayersAsync(_authenticationService.Token);
    }

    public Task<IApiResponse<object>> DeletePlayerAsync(string id)
    {
        return _api.DeletePlayerAsync(id, _authenticationService.Token);
    }
}
