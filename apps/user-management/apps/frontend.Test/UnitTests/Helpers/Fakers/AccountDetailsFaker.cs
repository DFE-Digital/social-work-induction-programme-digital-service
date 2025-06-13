using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class AccountDetailsFaker : Faker<AccountDetails>
{
    public AccountDetailsFaker()
    {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.MiddleNames, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.Email, f => f.Internet.Email());
        RuleFor(a => a.SocialWorkEnglandNumber, f => f.Random.Number(1, 1000).ToString());
        RuleFor(a => a.ProgrammeStartDate, f => DateOnly.FromDateTime(f.Date.Past()));
        RuleFor(a => a.ProgrammeEndDate, f => DateOnly.FromDateTime(f.Date.Future()));
    }
}

public static class AccountDetailsFakerExtensions
{
    public static AccountDetails GenerateWithInvalidEmail(
        this AccountDetailsFaker accountDetailsFaker,
        string? email = null
    )
    {
        return accountDetailsFaker.RuleFor(a => a.Email, _ => email).Generate();
    }

    public static AccountDetails GenerateWithCustomName(
        this AccountDetailsFaker accountDetailsFaker,
        string? firstName,
        string? lastName
    )
    {
        return accountDetailsFaker
            .RuleFor(a => a.FirstName, _ => firstName)
            .RuleFor(a => a.LastName, _ => lastName)
            .Generate();
    }

    public static AccountDetails GenerateWithSweId(
        this AccountDetailsFaker accountDetailsFaker,
        string? sweId
    )
    {
        return accountDetailsFaker.RuleFor(a => a.SocialWorkEnglandNumber, _ => sweId).Generate();
    }
}
