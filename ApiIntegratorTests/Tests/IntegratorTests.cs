using System.Net;
using ApiIntegratorTests.Extensions;
using ApiIntegratorTests.Fakers;
using ApiIntegratorTests.Fixtures;
using ApiIntegratorTests.Generated;
using ApiIntegratorTests.Interfaces;
using ApiIntegratorTests.Json.Appsettings;
using Microsoft.Extensions.Options;
using Refit;
using Shouldly;

namespace ApiIntegratorTests.Tests;

[Collection(nameof(CreateResourcesCollection))]
[TestCaseOrderer(typeof(TestCaseOrderer))]
public class IntegratorTests
{
    private readonly IIntegratorAutomationApi _api;
    private readonly IAuthApi _authApi;
    private readonly IntegratorApiConfig _settings;
    private readonly CreateResoucesFixture _createdResourceFixture;
    private readonly PlayerCreateRequestFaker _playerFaker;

    public IntegratorTests(IIntegratorAutomationApi api, IAuthApi authApi, IOptions<IntegratorApiConfig> settings, CreateResoucesFixture createdResourceFixture)
    {
        _api = api;
        _authApi = authApi;
        _settings = settings.Value;
        _createdResourceFixture = createdResourceFixture;
        _playerFaker = new PlayerCreateRequestFaker();
    }

    [Fact, TestOrder(1)]
    public async Task Login_Returns200_AndContainsAccessToken()
    {
        // Arrange
        var credentials = new CredentialsDTO
        {
            Email = _settings.Email,
            Password = _settings.Password,
        };

        // Act
        var loginResponse = await _authApi.LoginAsync(credentials);

        // Assert
        loginResponse.ShouldNotBeNull();
        loginResponse.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact, TestOrder(2)]
    public async Task CreatePlayers_Returns201_AndMatchesSpec()
    {
        // Arrange
        var createRequests = _playerFaker.Generate(12);

        // Act
        var createResponses = new List<IApiResponse<PlayerResponseDTO>>();
        foreach (var request in createRequests)
        {
            createResponses.Add(await _api.CreatePlayerAsync(request));
        }

        // Assert
        createResponses.Count.ShouldBe(12);
        for (var i = 0; i < createRequests.Count; i++)
        {
            var createResponse = createResponses[i];
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            createResponse.Content.ShouldNotBeNull();
            createResponse.Content.ShouldBe(createRequests[i]);

            _createdResourceFixture.AddCreatePlayer(createResponse.Content);
        }
    }

    [Fact, TestOrder(3)]
    public async Task GetPlayerByEmail_Returns200_AndMatchesSpec()
    {
        // Arrange
        var createRequest = _playerFaker.Generate();

        // Act
        var createResponse = await _api.CreatePlayerAsync(createRequest);
        var getOneResponse = await _api.GetPlayerByEmailAsync(new PlayerRequestOneDTO
        {
            Email = createResponse.Content?.Email ?? throw new InvalidOperationException("Email is empty"),
        });

        // Assert
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        createResponse.Content.ShouldNotBeNull();
        var createdPlayer = createResponse.Content;
        createdPlayer.ShouldBe(createRequest);
        _createdResourceFixture.AddCreatePlayer(createdPlayer);

        getOneResponse.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
        getOneResponse.Content.ShouldNotBeNull();
        var retrievedPlayer = getOneResponse.Content;
        retrievedPlayer.ShouldBe(createdPlayer);
    }

    [Fact, TestOrder(4)]
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

        var sortedPlayers = allPlayers.OrderBy(p => p.Name).ToArray();
        sortedPlayers.ShouldNotBeEmpty();

        for (var i = 0; i < createRequests.Count - 1; i++)
        {
            var firstIndex = Array.FindIndex(sortedPlayers, p => p.Name == createRequests[i].Name && p.Email == createRequests[i].Email);
            var secondIndex = Array.FindIndex(sortedPlayers, p => p.Name == createRequests[i + 1].Name && p.Email == createRequests[i + 1].Email);
            firstIndex.ShouldBeGreaterThanOrEqualTo(0);
            secondIndex.ShouldBeGreaterThanOrEqualTo(0);
            firstIndex.ShouldBeLessThan(secondIndex);
        }
    }

    [Fact, TestOrder(5)]
    public async Task DeleteAllPlayers_ReturnsEmptyList()
    {
        // Act
        var getAllResponse = await _api.GetAllPlayersAsync();
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        getAllResponse.Content.ShouldNotBeNull();

        var allPlayers = getAllResponse.Content;
        foreach (var player in allPlayers)
        {
            var deleteResponse = await _api.DeletePlayerAsync(player.Id);
            deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var verifyEmptyResponse = await _api.GetAllPlayersAsync();

        // Assert
        verifyEmptyResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        verifyEmptyResponse.Content.ShouldNotBeNull();
        verifyEmptyResponse.Content.Length.ShouldBe(0);
    }
}
