using ApiIntegratorTests.Generated;
using Refit;

namespace ApiIntegratorTests.Interfaces;

public interface IIntegratorAutomationApi
{
    [Post("/api/automationTask/create")]
    Task<PlayerResponseDTO> CreatePlayerAsync([Body] PlayerRequestDTO request);

    [Post("/api/automationTask/getOne")]
    Task<PlayerResponseDTO> GetPlayerByEmailAsync([Body] PlayerRequestOneDTO request);

    [Get("/api/automationTask/getAll")]
    Task<PlayerResponseDTO[]> GetAllPlayersAsync();

    [Delete("/api/automationTask/deleteOne/{id}")]
    Task<PlayerResponseDTO> DeletePlayerAsync(int id);
}
