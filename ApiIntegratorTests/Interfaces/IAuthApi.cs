using ApiIntegratorTests.Generated;
using Refit;

namespace ApiIntegratorTests.Interfaces;

public interface IAuthApi
{
    [Post("/api/tester/login")]
    Task<TokenDTO> LoginAsync([Body] CredentialsDTO request);
}
