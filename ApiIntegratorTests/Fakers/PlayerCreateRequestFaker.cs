using ApiIntegratorTests.Generated;
using Bogus;

namespace ApiIntegratorTests.Fakers;

public sealed class PlayerCreateRequestFaker : Faker<PlayerRequestDTO>
{
    public PlayerCreateRequestFaker()
    {
        RuleFor(x => x.Username, f => $"TEST_player_{f.Random.Guid().ToString("N")[..8]}");
        RuleFor(x => x.Password_change, f => $"P@ssw0rd!{f.Random.Int(10, 99)}");
        RuleFor(x => x.Password_repeat, (f, x) => x.Password_change);
        RuleFor(x => x.Email, (f, x) => $"{x.Username}@example.com");
        RuleFor(x => x.Name, f => f.Name.FirstName());
        RuleFor(x => x.Surname, f => f.Name.LastName());
        RuleFor(x => x.Currency_code, "EUR");
    }
}
