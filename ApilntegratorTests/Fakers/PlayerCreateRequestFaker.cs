using ApilntegratorTests.Generated;
using Bogus;

namespace ApilntegratorTests.Fakers;

public sealed class PlayerCreateRequestFaker : Faker<PlayerCreateRequest>
{
    public PlayerCreateRequestFaker()
    {
        RuleFor(x => x.Username, f => $"player_{f.Random.Guid().ToString("N")[..8]}");
        RuleFor(x => x.Password, f => $"P@ssw0rd!{f.Random.Int(10, 99)}");
        RuleFor(x => x.Email, (f, x) => $"{x.Username}@example.com");
        RuleFor(x => x.Name, f => f.Name.FirstName());
        RuleFor(x => x.Surname, f => f.Name.LastName());
        RuleFor(x => x.Currency, "EUR");
    }
}
