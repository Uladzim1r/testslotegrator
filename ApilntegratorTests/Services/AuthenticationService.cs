using ApilntegratorTests.Interfaces;
using ApilntegratorTests.Json.Appsettings;
using ApilntegratorTests.Models;

namespace ApilntegratorTests.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAutomationApi _api;
    private readonly ApiSetting _settings;

    public string Token { get; private set; } = string.Empty;

    public AuthenticationService(IAutomationApi api, ApiSetting settings)
    {
        _api = api;
        _settings = settings;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var response = await _api.LoginAsync(new LoginRequest
        {
            Username = _settings.Username,
            Password = _settings.Password
        });

        if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(response.Content?.EffectiveToken))
        {
            throw new InvalidOperationException(
                $"Login failed. Status: {response.StatusCode}, Error: {response.Error?.Message}");
        }

        Token = $"Bearer {response.Content.EffectiveToken}";
    }
}
