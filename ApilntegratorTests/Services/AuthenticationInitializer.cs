using Microsoft.Extensions.Hosting;

namespace ApilntegratorTests.Services;

public sealed class AuthenticationInitializer : IHostedService
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationInitializer(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _authenticationService.InitializeAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
