using ApilntegratorTests.Json.Appsettings;
using ApilntegratorTests.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ApilntegratorTests.Services;

public sealed class AuthorizationHandler : DelegatingHandler
{
    private readonly ApiSetting _settings;
    private readonly ILogger<AuthorizationHandler> _logger;
    private readonly Lazy<Task<string>> _tokenLazy;

    public AuthorizationHandler(ApiSetting settings, ILogger<AuthorizationHandler> logger)
    {
        _settings = settings;
        _logger = logger;
        _tokenLazy = new Lazy<Task<string>>(GetTokenAsync);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Attaching authorization header to {Method} {RequestUri}", request.Method, request.RequestUri);
        var token = await _tokenLazy.Value;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetTokenAsync()
    {
        if (InnerHandler is null)
        {
            throw new InvalidOperationException($"{nameof(AuthorizationHandler)}.{nameof(InnerHandler)} must be set before sending requests.");
        }

        _logger.LogInformation("Requesting authentication token from {LoginEndpoint}", "/api/tester/login");

        using var loginClient = new HttpClient(InnerHandler, disposeHandler: false)
        {
            BaseAddress = new Uri(_settings.BaseUrl)
        };

        var loginRequest = new LoginRequest
        {
            Username = _settings.Username,
            Password = _settings.Password
        };

        var response = await loginClient.PostAsJsonAsync("/api/tester/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        var token = loginResponse?.EffectiveToken;

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Login response did not contain a token.");
        }

        _logger.LogInformation("Authentication token obtained successfully");
        return token;
    }
}
