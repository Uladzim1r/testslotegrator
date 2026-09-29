using System.Net;
using ApiIntegratorTests.Extensions;
using ApiIntegratorTests.Fakers;
using ApiIntegratorTests.Fixtures;
using ApiIntegratorTests.Generated;
using ApiIntegratorTests.Interfaces;
using Refit;
using Shouldly;

namespace ApiIntegratorTests.Tests;

[Collection(nameof(CreateResourcesCollection))]
[TestCaseOrderer(typeof(TestCaseOrderer))]
public class IntegratorTests
{
    private const int PlayersCount = 12;

    private readonly IIntegratorAutomationApi _api;
    private readonly CreateResoucesFixture _createdResourceFixture;
    private readonly PlayerCreateRequestFaker _playerFaker;

    public IntegratorTests(IIntegratorAutomationApi api, CreateResoucesFixture createdResourceFixture)
    {
        _api = api;
        _createdResourceFixture = createdResourceFixture;
        _playerFaker = new PlayerCreateRequestFaker();
    }

    [Fact, TestOrder(1)]
    public async Task CreatePlayers_Returns201_AndMatchesSpec()
    {
        // Arrange
        var createRequests = _playerFaker.Generate(PlayersCount);

        // Act
        var createResponses = new List<IApiResponse<PlayerResponseDTO>>();
        foreach (var request in createRequests)
        {
            var playerAsync = await _api.CreatePlayerAsync(request);
            createResponses.Add(playerAsync);
        }

        // Assert
        createResponses.Count.ShouldBe(PlayersCount);
        for (var i = 0; i < createRequests.Count; i++)
        {
            var createResponse = createResponses[i];
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            createResponse.Content.ShouldNotBeNull();
            createResponse.Content.ShouldBe(createRequests[i]);

            _createdResourceFixture.AddCreatePlayer(createResponse.Content);
        }
    }

    [Fact, TestOrder(2)]
    public async Task GetPlayerByEmail_Returns200_AndMatchesSpec()
    {
        // Arrange
        var createRequest = _playerFaker.Generate();

        // Act
        var createResponse = await _api.CreatePlayerAsync(createRequest);
        var getOneResponse = await _api.GetPlayerByEmailAsync(new PlayerRequestOneDTO
        {
            Email = createResponse.Content?.Email ?? string.Empty
        });

        // Assert
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        createResponse.Content.ShouldNotBeNull();
        var createdPlayer = createResponse.Content;
        createdPlayer.ShouldBe(createRequest);
        _createdResourceFixture.AddCreatePlayer(createdPlayer);

        getOneResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        getOneResponse.Content.ShouldNotBeNull();
        var retrievedPlayer = getOneResponse.Content;
        retrievedPlayer.ShouldBe(createdPlayer);
    }

    [Fact, TestOrder(3)]
    public async Task GetAllPlayers_Returns200_SortedByName()
    {
        // Arrange
        var createRequests = _playerFaker.Generate(3);
        createRequests[0].Name = "Alice";
        createRequests[1].Name = "Bob";
        createRequests[2].Name = "Charlie";

        // Act
        var createResponses = new List<IApiResponse<PlayerResponseDTO>>();
        foreach (var request in createRequests)
        {
            createResponses.Add(await _api.CreatePlayerAsync(request));
        }

        var getAllResponse = await _api.GetAllPlayersAsync();

        // Assert
        foreach (var createResponse in createResponses)
        {
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            createResponse.Content.ShouldNotBeNull();
            _createdResourceFixture.AddCreatePlayer(createResponse.Content);
        }

        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        getAllResponse.Content.ShouldNotBeNull();

        var allPlayers = getAllResponse.Content;
        allPlayers.Length.ShouldBeGreaterThanOrEqualTo(createRequests.Count);

        var sortedByName = allPlayers.OrderBy(p => p.Name).ToArray();
        allPlayers.ShouldBe(sortedByName);
    }
}
