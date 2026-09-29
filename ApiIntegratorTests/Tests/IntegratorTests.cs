using ApiIntegratorTests.Fixtures;
using ApiIntegratorTests.Generated;
using ApiIntegratorTests.Interfaces;
using Shouldly;

namespace ApiIntegratorTests.Tests;

[Collection("Players")]
public class IntegratorTests
{
    private readonly IIntegratorAutomationApi _api;
    private readonly CreateResoucesFixture _fixture;

    public IntegratorTests(IIntegratorAutomationApi api, CreateResoucesFixture fixture)
    {
        _api = api;
        _fixture = fixture;
    }

    [Fact]
    public async Task FullAutomationFlow()
    {
        // Fixture has already created 12 players.
        _fixture.Players.Count.ShouldBe(12);

        foreach (var player in _fixture.Players)
        {
            player.ShouldNotBeNull("Create player response should contain a player object.");
            player.Id.ShouldBeGreaterThan(0, "Created player should have a positive id.");
            player.Username.ShouldNotBeNullOrWhiteSpace("Created player should have a username.");
            player.Email.ShouldNotBeNullOrWhiteSpace("Created player should have an email.");
            player.Name.ShouldNotBeNullOrWhiteSpace("Created player should have a name.");
            player.Surname.ShouldNotBeNullOrWhiteSpace("Created player should have a surname.");
        }

        // Verify the created players are present in the full list.
        var allPlayers = await _api.GetAllPlayersAsync();

        allPlayers.ShouldNotBeNull("GetAll response should contain a list of players.");
        allPlayers.ShouldNotBeEmpty("GetAll should return at least one player.");

        var createdPlayerIds = _fixture.Players.Select(p => p.Id).ToHashSet();
        var foundCreatedPlayers = allPlayers.Where(p => createdPlayerIds.Contains(p.Id)).ToList();
        foundCreatedPlayers.Count.ShouldBe(12, "All 12 created players should be present in GetAll response.");
    }

    [Fact]
    public async Task GetPlayerByEmail_ShouldReturnProfile()
    {
        var created = _fixture.Players.First();

        var player = await _api.GetPlayerByEmailAsync(new PlayerRequestOneDTO { Email = created.Email });

        player.ShouldNotBeNull();
        player.Id.ShouldBe(created.Id);
        player.Username.ShouldBe(created.Username);
        player.Email.ShouldBe(created.Email);
        player.Name.ShouldBe(created.Name);
        player.Surname.ShouldBe(created.Surname);
    }
}
