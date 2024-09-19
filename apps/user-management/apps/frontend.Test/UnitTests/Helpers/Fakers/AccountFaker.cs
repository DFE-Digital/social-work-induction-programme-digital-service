using System.Collections.Immutable;
using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class AccountFaker : Faker<Account>
{
    public AccountFaker()
    {
        RuleFor(a => a.Id, f => f.Random.Guid());
        RuleFor(a => a.CreatedAt, f => f.Date.Past());
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.Status, f => f.PickRandom<AccountStatus>());
        RuleFor(a => a.Email, f => f.Internet.Email());
        RuleFor(a => a.Types, f => [f.PickRandom<AccountType>()]);
        RuleFor(
            a => a.SocialWorkEnglandNumber,
            (f, current) =>
                current.Types?.Contains(AccountType.EarlyCareerSocialWorker) == true
                    ? $"SW{f.Random.Number(1, 1000)}"
                    : null
        );
    }
}

public static class AccountFakerExtensions
{
    public static Account GenerateNewAccount(this AccountFaker accountFaker)
    {
        return accountFaker.RuleFor(a => a.Status, _ => AccountStatus.Active).Generate();
    }

    private static Faker<Account> GetSocialWorkerAccountFaker(this AccountFaker accountFaker)
    {
        return accountFaker.RuleFor(a => a.Types, _ => [AccountType.EarlyCareerSocialWorker]);
    }

    public static Account GenerateSocialWorkerWithNoSweNumber(this AccountFaker accountFaker)
    {
        return accountFaker
            .GetSocialWorkerAccountFaker()
            .RuleFor(a => a.SocialWorkEnglandNumber, _ => null)
            .Generate();
    }

    public static Account GenerateSocialWorker(this AccountFaker accountFaker)
    {
        return accountFaker.GetSocialWorkerAccountFaker().Generate();
    }

    public static Account GenerateAccountWithTypes(
        this AccountFaker accountFaker,
        params AccountType[] accountTypes
    )
    {
        return accountFaker.RuleFor(a => a.Types, _ => accountTypes.ToImmutableList()).Generate();
    }
}
