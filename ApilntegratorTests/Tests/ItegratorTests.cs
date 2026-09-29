using ApilntegratorTests.Fakers;
using ApilntegratorTests.Generated;
using ApilntegratorTests.Interfaces;
using Shouldly;
using System.Text.Json;

namespace ApilntegratorTests.Tests;

public class ItegratorTests
{
    private readonly IItegratorAutomationApi _api;

    public ItegratorTests(IItegratorAutomationApi api)
    {
        _api = api;
    }

    [Fact]
    public async Task FullAutomationFlow()
    {
        // Authorization is handled transparently by AuthorizationHandler.

        // Step 1: Register 12 players and validate the response properties.
        var playerFaker = new PlayerCreateRequestFaker();
        var createdPlayers = new List<PlayerResponse>();
        for (int i = 0; i < 12; i++)
        {
            var request = playerFaker.Generate();

            var created = await _api.CreatePlayerAsync(request);

            created.ShouldNotBeNull("Create player response should contain a player object.");
            created.Id.ShouldNotBeNullOrWhiteSpace("Created player should have a non-empty id.");
            created.Username.ShouldBe(request.Username, "Created player username should match the request.");
            created.Email.ShouldBe(request.Email, "Created player email should match the request.");
            created.Name.ShouldBe(request.Name, "Created player name should match the request.");
            created.Surname.ShouldBe(request.Surname, "Created player surname should match the request.");
            created.Currency.ShouldBe(request.Currency, "Created player currency should match the request.");

            createdPlayers.Add(created);
        }

        createdPlayers.Count.ShouldBe(12);

        // Step 2: Retrieve profile data for a created player and validate its shape.
        var profile = await _api.GetPlayerAsync();

        profile.ShouldNotBeNull("GetOne response should contain a player profile.");
        profile.Id.ShouldNotBeNullOrWhiteSpace("Player profile should have a non-empty id.");
        profile.Username.ShouldNotBeNullOrWhiteSpace("Player profile should have a username.");
        profile.Email.ShouldNotBeNullOrWhiteSpace("Player profile should have an email.");
        profile.Name.ShouldNotBeNullOrWhiteSpace("Player profile should have a name.");
        profile.Surname.ShouldNotBeNullOrWhiteSpace("Player profile should have a surname.");
        profile.Currency.ShouldNotBeNullOrWhiteSpace("Player profile should have a currency.");

        // Step 3: Retrieve all users and verify content plus sorting by name.
        var playersList = await _api.GetAllPlayersAsync();

        playersList.ShouldNotBeNull("GetAll response should contain a list of players.");

        var allPlayers = playersList.Players;
        allPlayers.ShouldNotBeEmpty("GetAll should return at least one player.");

        var createdPlayerIds = createdPlayers.Select(p => p.Id).ToHashSet();
        var foundCreatedPlayers = allPlayers.Where(p => createdPlayerIds.Contains(p.Id)).ToList();
        foundCreatedPlayers.Count.ShouldBe(12, "All 12 created players should be present in GetAll response.");

        var sortedByName = allPlayers
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        allPlayers.SequenceEqual(sortedByName).ShouldBeTrue(
            $"Players should be sorted by name. Actual order: {JsonSerializer.Serialize(allPlayers.Select(p => p.Name))}");

        // Step 4: Delete all previously created users and verify removal.
        foreach (var player in createdPlayers)
        {
            await _api.DeletePlayerAsync(player.Id);
        }

        var playersListAfterDelete = await _api.GetAllPlayersAsync();
        playersListAfterDelete.ShouldNotBeNull();

        var remainingPlayerIds = playersListAfterDelete.Data
            .Select(p => p.Id)
            .ToHashSet();

        foreach (var player in createdPlayers)
        {
            remainingPlayerIds.ShouldNotContain(
                player.Id,
                $"Deleted player {player.Id} should no longer appear in GetAll response.");
        }
    }
}
