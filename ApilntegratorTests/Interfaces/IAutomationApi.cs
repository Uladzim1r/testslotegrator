using ApilntegratorTests.Generated;
using Refit;

namespace ApilntegratorTests.Interfaces;

public interface IAutomationApi
{
    [Post("/api/tester/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request);

    [Post("/api/automationTask/create")]
    Task<IApiResponse<PlayerResponse>> CreatePlayerAsync([Body] PlayerCreateRequest request);

    [Get("/api/automationTask/getOne")]
    Task<IApiResponse<PlayerResponse>> GetPlayerAsync();

    [Get("/api/automationTask/getAll")]
    Task<IApiResponse<PlayersListResponse>> GetAllPlayersAsync();

    [Delete("/api/automationTask/deleteOne/{id}")]
    Task<IApiResponse<object>> DeletePlayerAsync(string id);
}
