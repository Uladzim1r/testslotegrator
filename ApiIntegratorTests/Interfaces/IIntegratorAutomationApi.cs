using ApiIntegratorTests.Generated;
using Refit;

namespace ApiIntegratorTests.Interfaces;

public interface IIntegratorAutomationApi
{
    [Post("/api/automationTask/create")]
    Task<IApiResponse<PlayerResponseDTO>> CreatePlayerAsync([Body] PlayerRequestDTO request);

    [Post("/api/automationTask/getOne")]
    Task<IApiResponse<PlayerResponseDTO>> GetPlayerByEmailAsync([Body] PlayerRequestOneDTO request);

    [Get("/api/automationTask/getAll")]
    Task<IApiResponse<PlayerResponseDTO[]>> GetAllPlayersAsync();

    [Delete("/api/automationTask/deleteOne/{id}")]
    Task<PlayerResponseDTO> DeletePlayerAsync(int id);
}
