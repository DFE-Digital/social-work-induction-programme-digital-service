using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class UserDetailsFaker : Faker<UserDetails>
{
    public UserDetailsFaker()
    {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.Email, f => f.Internet.Email());
        RuleFor(a => a.SocialWorkEnglandNumber, f => f.Random.Number(1, 1000).ToString());
    }
}

public static class UserDetailsFakerExtensions
{
    public static UserDetails GenerateWithInvalidEmail(
        this UserDetailsFaker userDetailsFaker,
        string? email = null
    )
    {
        return userDetailsFaker.RuleFor(a => a.Email, _ => email).Generate();
    }

    public static UserDetails GenerateWithCustomName(
        this UserDetailsFaker userDetailsFaker,
        string? firstName,
        string? lastName
    )
    {
        return userDetailsFaker
            .RuleFor(a => a.FirstName, _ => firstName)
            .RuleFor(a => a.LastName, _ => lastName)
            .Generate();
    }

    public static UserDetails GenerateWithSweId(
        this UserDetailsFaker userDetailsFaker,
        string? sweId
    )
    {
        return userDetailsFaker.RuleFor(a => a.SocialWorkEnglandNumber, _ => sweId).Generate();
    }
}
