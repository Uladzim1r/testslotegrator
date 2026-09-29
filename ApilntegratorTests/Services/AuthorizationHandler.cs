using ApilntegratorTests.Generated;
using ApilntegratorTests.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using ApilntegratorTests.Json.Appsettings;
using Microsoft.Extensions.Options;

namespace ApilntegratorTests.Services;

public sealed class AuthorizationHandler : DelegatingHandler
{
    private readonly IntegratorApiConfig _settings;
    private readonly IAuthApi _authApi;
    private readonly ILogger<AuthorizationHandler> _logger;
    private readonly Lazy<Task<string>> _tokenLazy;

    public AuthorizationHandler(IOptions<IntegratorApiConfig> integratorApiConfig, IAuthApi authApi,
        ILogger<AuthorizationHandler> logger)
    {
        _settings = integratorApiConfig.Value;
        _authApi = authApi;
        _logger = logger;
        _tokenLazy = new Lazy<Task<string>>(GetTokenAsync);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Attaching authorization header to {Method} {RequestUri}", request.Method, request.RequestUri);
        var token = await _tokenLazy.Value;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetTokenAsync()
    {
        var loginRequest = new LoginRequest
        {
            Username = _settings.Username,
            Password = _settings.Password,
        };

        var loginResponse = await _authApi.LoginAsync(loginRequest);
        var token = loginResponse.Access_token;

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Login response did not contain a token.");
        }

        _logger.LogInformation("Authentication token obtained successfully");
        return token;
    }
}