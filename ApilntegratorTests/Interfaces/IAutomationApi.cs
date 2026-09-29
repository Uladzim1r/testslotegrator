using ApilntegratorTests.Models;
using Refit;

namespace ApilntegratorTests.Interfaces;

public interface IAutomationApi
{
    [Post("/api/tester/login")]
    Task<IApiResponse<LoginResponse>> LoginAsync([Body] LoginRequest request);

    [Post("/api/automationTask/create")]
    Task<IApiResponse<PlayerResponse>> CreatePlayerAsync([Body] PlayerCreateRequest request, [Header("Authorization")] string authorization);

    [Get("/api/automationTask/getOne")]
    Task<IApiResponse<PlayerResponse>> GetPlayerAsync([Header("Authorization")] string authorization);

    [Get("/api/automationTask/getAll")]
    Task<IApiResponse<PlayersListResponse>> GetAllPlayersAsync([Header("Authorization")] string authorization);

    [Delete("/api/automationTask/deleteOne/{id}")]
    Task<IApiResponse<object>> DeletePlayerAsync(string id, [Header("Authorization")] string authorization);
}
