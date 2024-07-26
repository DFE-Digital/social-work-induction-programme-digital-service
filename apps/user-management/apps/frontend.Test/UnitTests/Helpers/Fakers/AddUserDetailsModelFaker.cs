using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class AddUserDetailsModelFaker : Faker<AccountDetails>
{
    public AddUserDetailsModelFaker()
    {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.Email, f => f.Internet.Email());
        RuleFor(a => a.SocialWorkEnglandNumber, f => f.Random.Number().ToString());
    }
}

public static class AddUserDetailsModelFakerExtensions
{
    public static AccountDetails GenerateWithInvalidEmail(
        this AddUserDetailsModelFaker addUserDetailsModelFaker,
        string? email = null
    )
    {
        return addUserDetailsModelFaker.RuleFor(a => a.Email, _ => email).Generate();
    }

    public static AccountDetails GenerateWithCustomName(
        this AddUserDetailsModelFaker addUserDetailsModelFaker,
        string? firstName,
        string? lastName
    )
    {
        return addUserDetailsModelFaker
            .RuleFor(a => a.FirstName, _ => firstName)
            .RuleFor(a => a.LastName, _ => lastName)
            .Generate();
    }
}
