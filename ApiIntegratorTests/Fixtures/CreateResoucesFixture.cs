using System.Collections.Concurrent;
using ApiIntegratorTests.Generated;
using ApiIntegratorTests.Interfaces;

namespace ApiIntegratorTests.Fixtures;

public sealed class CreateResoucesFixture : IAsyncLifetime
{
    private readonly IIntegratorAutomationApi _api;

    public CreateResoucesFixture(IIntegratorAutomationApi api)
    {
        _api = api;
    }

    private ConcurrentBag<PlayerResponseDTO> Players { get; } = new();

    public void AddCreatePlayer(PlayerResponseDTO player)
    {
        Players.Add(player);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var player in Players)
        {
            try
            {
                await _api.DeletePlayerAsync(player.Id);
            }
            catch
            {
                // Best-effort cleanup; do not fail disposal.
            }
        }
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}