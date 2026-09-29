using ApilntegratorTests.Generated;
using Refit;

namespace ApilntegratorTests.Interfaces;

public interface IAuthApi
{
    [Post("/api/tester/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request);
}
