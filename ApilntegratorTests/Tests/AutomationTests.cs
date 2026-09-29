using ApilntegratorTests.Fakers;
using ApilntegratorTests.Generated;
using ApilntegratorTests.Services;
using Shouldly;
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

            response.StatusCode.ShouldBe(
                HttpStatusCode.Created,
                $"Create player should return 201. Actual: {response.StatusCode}, Error: {response.Error?.Message}");
            response.Content.ShouldNotBeNull();
            response.Content.EffectiveId.ShouldNotBeNullOrWhiteSpace("Created player response should contain an id.");
            response.Content.Username.ShouldNotBeNullOrWhiteSpace("Created player response should contain a username.");

            createdPlayers.Add(response.Content);
        }

        createdPlayers.Count.ShouldBe(12);

        // Step 2: Retrieve profile data for a created player.
        var getOneResponse = await _api.GetPlayerAsync();

        getOneResponse.StatusCode.ShouldBe(
            HttpStatusCode.OK,
            $"GetOne should return 200. Actual: {getOneResponse.StatusCode}, Error: {getOneResponse.Error?.Message}");
        getOneResponse.Content.ShouldNotBeNull();

        // Step 3: Retrieve all users and verify they are sorted by name.
        var getAllResponse = await _api.GetAllPlayersAsync();

        getAllResponse.StatusCode.ShouldBe(
            HttpStatusCode.OK,
            $"GetAll should return 200. Actual: {getAllResponse.StatusCode}, Error: {getAllResponse.Error?.Message}");
        getAllResponse.Content.ShouldNotBeNull();

        var allPlayers = getAllResponse.Content.EffectiveItems;
        allPlayers.ShouldNotBeEmpty();

        var sortedByName = allPlayers
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        allPlayers.SequenceEqual(sortedByName).ShouldBeTrue(
            $"Players should be sorted by name. Actual order: {JsonSerializer.Serialize(allPlayers.Select(p => p.Name))}");

        // Step 4: Delete all previously created users.
        foreach (var player in createdPlayers)
        {
            var deleteResponse = await _api.DeletePlayerAsync(player.EffectiveId);

            (deleteResponse.StatusCode == HttpStatusCode.OK || deleteResponse.StatusCode == HttpStatusCode.NoContent)
                .ShouldBeTrue(
                    $"Delete player should return 200 or 204. Actual: {deleteResponse.StatusCode}, Error: {deleteResponse.Error?.Message}");
        }
    }
}
