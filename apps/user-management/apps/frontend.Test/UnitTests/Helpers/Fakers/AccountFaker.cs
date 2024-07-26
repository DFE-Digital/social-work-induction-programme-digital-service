using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class AccountFaker : Faker<Account>
{
    public AccountFaker()
    {
        RuleFor(a => a.Id, f => f.Random.Guid());
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.Status, f => f.PickRandom<AccountStatus>());
        RuleFor(a => a.Email, f => f.Internet.Email());
        RuleFor(a => a.Types, f => [f.PickRandom<AccountType>()]);
    }
}

public static class AccountFakerExtensions
{
    public static Account GenerateNewAccount(this AccountFaker accountFaker)
    {
        return accountFaker.RuleFor(a => a.Status, _ => AccountStatus.Active).Generate();
    }
}
