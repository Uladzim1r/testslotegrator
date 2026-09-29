using ApilntegratorTests.Interfaces;
using ApilntegratorTests.Json.Appsettings;
using ApilntegratorTests.Models;
using Microsoft.Extensions.Configuration;
using Refit;

namespace ApilntegratorTests.Fixtures;

public sealed class ApiTestFixture : IAsyncLifetime
{
    public ApiSetting Settings { get; private set; } = null!;
    public IAutomationApi Api { get; private set; } = null!;
    public string Token { get; private set; } = string.Empty;

    public ApiTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.local.json", optional: true)
            .Build();

        Settings = Appsetting.FromConfig(configuration).ApiSettings ?? new ApiSetting();
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
