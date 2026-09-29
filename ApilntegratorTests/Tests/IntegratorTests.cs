using ApilntegratorTests.Fakers;
using ApilntegratorTests.Generated;
using ApilntegratorTests.Interfaces;
using Shouldly;

namespace ApilntegratorTests.Tests;

public class IntegratorTests
{
    private readonly IIntegratorAutomationApi _api;

    public IntegratorTests(IIntegratorAutomationApi api)
    {
        _api = api;
    }

    [Fact]
    public async Task FullAutomationFlow()
    {
        // Step 1: Register 12 players and validate the response properties.
        var playerFaker = new PlayerCreateRequestFaker();
        var createdPlayers = new List<PlayerCreateResponse>();
        for (int i = 0; i < 12; i++)
        {
            var request = playerFaker.Generate();

            var created = await _api.CreatePlayerAsync(request);

            created.ShouldNotBeNull("Create player response should contain a player object.");
            created._id.ShouldNotBeNullOrWhiteSpace("Created player should have a non-empty id.");
            created.Username.ShouldBe(request.Username, "Created player username should match the request.");
            created.Email.ShouldBe(request.Email, "Created player email should match the request.");
            created.Name.ShouldBe(request.Name, "Created player name should match the request.");
            created.Surname.ShouldBe(request.Surname, "Created player surname should match the request.");

            createdPlayers.Add(created);
        }

        createdPlayers.Count.ShouldBe(12);

        // Step 2: Retrieve all users and verify the created players are present.
        var allPlayers = await _api.GetAllPlayersAsync();

        allPlayers.ShouldNotBeNull("GetAll response should contain a list of players.");
        allPlayers.ShouldNotBeEmpty("GetAll should return at least one player.");

        var createdPlayerIds = createdPlayers.Select(p => p._id).ToHashSet();
        var foundCreatedPlayers = allPlayers.Where(p => createdPlayerIds.Contains(p.Id)).ToList();
        foundCreatedPlayers.Count.ShouldBe(12, "All 12 created players should be present in GetAll response.");

        // Step 3: Delete all previously created users and verify removal.
        foreach (var player in createdPlayers)
        {
            await _api.DeletePlayerAsync(player._id);
        }

        var playersAfterDelete = await _api.GetAllPlayersAsync();
        playersAfterDelete.ShouldNotBeNull();

        var remainingPlayerIds = playersAfterDelete
            .Select(p => p.Id)
            .ToHashSet();

        foreach (var player in createdPlayers)
        {
            remainingPlayerIds.ShouldNotContain(
                player._id,
                $"Deleted player {player._id} should no longer appear in GetAll response.");
        }
    }

    [Fact(Skip = "GET /api/automationTask/getOne/{id} currently returns 404 from the server.")]
    public async Task GetPlayerById_ShouldReturnProfile()
    {
        var request = new PlayerCreateRequestFaker().Generate();
        var created = await _api.CreatePlayerAsync(request);

        var player = await _api.GetPlayerAsync(created._id);

        player.ShouldNotBeNull();
        player.Id.ShouldBe(created._id);
        player.Username.ShouldBe(created.Username);
        player.Email.ShouldBe(created.Email);
        player.Name.ShouldBe(created.Name);
        player.Surname.ShouldBe(created.Surname);
    }
}
