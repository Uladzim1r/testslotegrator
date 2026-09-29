namespace ApilntegratorTests.Services;

public interface IAuthenticationService
{
    string Token { get; }
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
