using ApiIntegratorTests.Generated;
using Shouldly;

namespace ApiIntegratorTests.Extensions;

public static class ValidationExtentions
{
    public static void ShouldBe(this PlayerResponseDTO actual, PlayerRequestDTO expected)
    {
        actual.ShouldNotBeNull();
        expected.ShouldNotBeNull();

        actual.Id.ShouldBeGreaterThan(0);
        actual.Username.ShouldBe(expected.Username);
        actual.Email.ShouldBe(expected.Email);
        actual.Name.ShouldBe(expected.Name);
        actual.Surname.ShouldBe(expected.Surname);
    }

    public static void ShouldBe(this PlayerResponseDTO actual, PlayerResponseDTO expected)
    {
        actual.ShouldNotBeNull();
        expected.ShouldNotBeNull();

        actual.Id.ShouldBe(expected.Id);
        actual.Username.ShouldBe(expected.Username);
        actual.Email.ShouldBe(expected.Email);
        actual.Name.ShouldBe(expected.Name);
        actual.Surname.ShouldBe(expected.Surname);
    }
}
