using ApilntegratorTests.Generated;
using Refit;

namespace ApilntegratorTests.Interfaces;

public interface IIntegratorAutomationApi
{
    [Post("/api/automationTask/create")]
    Task<PlayerCreateResponse> CreatePlayerAsync([Body] PlayerCreateRequest request);

    [Get("/api/automationTask/getOne/{id}")]
    Task<PlayerListItem> GetPlayerAsync(string id);

    [Get("/api/automationTask/getAll")]
    Task<PlayerListItem[]> GetAllPlayersAsync();

    [Delete("/api/automationTask/deleteOne/{id}")]
    Task<IApiResponse> DeletePlayerAsync(string id);
}
