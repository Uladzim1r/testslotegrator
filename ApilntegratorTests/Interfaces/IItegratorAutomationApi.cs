using ApilntegratorTests.Generated;
using Refit;

namespace ApilntegratorTests.Interfaces;

public interface IItegratorAutomationApi
{
    [Post("/api/automationTask/create")]
    Task<PlayerResponse> CreatePlayerAsync([Body] PlayerCreateRequest request);

    [Get("/api/automationTask/getOne")]
    Task<PlayerResponse> GetPlayerAsync();

    [Get("/api/automationTask/getAll")]
    Task<PlayersListResponse> GetAllPlayersAsync();

    [Delete("/api/automationTask/deleteOne/{id}")]
    Task<IApiResponse> DeletePlayerAsync(string id);
}
