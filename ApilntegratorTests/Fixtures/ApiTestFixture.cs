using ApilntegratorTests.Configuration;
using ApilntegratorTests.Interfaces;
using ApilntegratorTests.Models;
using Microsoft.Extensions.Configuration;
using Refit;

namespace ApilntegratorTests.Fixtures;

public sealed class ApiTestFixture : IAsyncLifetime
{
    public ApiSettings Settings { get; } = new();
    public IAutomationApi Api { get; private set; } = null!;
    public string Token { get; private set; } = string.Empty;

    public ApiTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        configuration.GetSection("ApiSettings").Bind(Settings);
    }

    public async ValueTask InitializeAsync()
    {
        Api = RestService.For<IAutomationApi>(Settings.BaseUrl);

        var loginResponse = await Api.LoginAsync(new LoginRequest
        {
            Username = Settings.Username,
            Password = Settings.Password
        });

        if (!loginResponse.IsSuccessStatusCode || string.IsNullOrWhiteSpace(loginResponse.Content?.EffectiveToken))
        {
            throw new InvalidOperationException(
                $"Login failed. Status: {loginResponse.StatusCode}, Error: {loginResponse.Error?.Message}");
        }

        Token = $"Bearer {loginResponse.Content.EffectiveToken}";
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
