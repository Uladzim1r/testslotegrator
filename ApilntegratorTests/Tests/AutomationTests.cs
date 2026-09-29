using ApilntegratorTests.Fakers;
using ApilntegratorTests.Models;
using ApilntegratorTests.Services;
using System.Net;
using System.Text.Json;
using Xunit;

namespace ApilntegratorTests.Tests;

public class AutomationTests
{
    private readonly IAutomationTaskApi _api;

    public AutomationTests(IAutomationTaskApi api)
    {
        _api = api;
    }

    [Fact]
    public async Task FullAutomationFlow()
    {
        // Authorization is handled transparently by AuthorizationHandler.

        // Step 1: Register 12 players.
        var playerFaker = new PlayerCreateRequestFaker();
        var createdPlayers = new List<PlayerResponse>();
        for (int i = 0; i < 12; i++)
        {
            var request = playerFaker.Generate();

            var response = await _api.CreatePlayerAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Create player should return 201. Actual: {response.StatusCode}, Error: {response.Error?.Message}");
            Assert.NotNull(response.Content);
            Assert.False(
                string.IsNullOrWhiteSpace(response.Content.EffectiveId),
                "Created player response should contain an id.");
            Assert.False(
                string.IsNullOrWhiteSpace(response.Content.Username),
                "Created player response should contain a username.");

            createdPlayers.Add(response.Content);
        }

        Assert.Equal(12, createdPlayers.Count);

        // Step 2: Retrieve profile data for a created player.
        var getOneResponse = await _api.GetPlayerAsync();

        Assert.True(
            getOneResponse.StatusCode == HttpStatusCode.OK,
            $"GetOne should return 200. Actual: {getOneResponse.StatusCode}, Error: {getOneResponse.Error?.Message}");
        Assert.NotNull(getOneResponse.Content);

        // Step 3: Retrieve all users and verify they are sorted by name.
        var getAllResponse = await _api.GetAllPlayersAsync();

        Assert.True(
            getAllResponse.StatusCode == HttpStatusCode.OK,
            $"GetAll should return 200. Actual: {getAllResponse.StatusCode}, Error: {getAllResponse.Error?.Message}");
        Assert.NotNull(getAllResponse.Content);

        var allPlayers = getAllResponse.Content.EffectiveItems;
        Assert.NotEmpty(allPlayers);

        var sortedByName = allPlayers
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        Assert.True(
            allPlayers.SequenceEqual(sortedByName),
            $"Players should be sorted by name. Actual order: {JsonSerializer.Serialize(allPlayers.Select(p => p.Name))}");

        // Step 4: Delete all previously created users.
        foreach (var player in createdPlayers)
        {
            var deleteResponse = await _api.DeletePlayerAsync(player.EffectiveId);

            Assert.True(
                deleteResponse.StatusCode == HttpStatusCode.OK || deleteResponse.StatusCode == HttpStatusCode.NoContent,
                $"Delete player should return 200 or 204. Actual: {deleteResponse.StatusCode}, Error: {deleteResponse.Error?.Message}");
        }
    }
}
