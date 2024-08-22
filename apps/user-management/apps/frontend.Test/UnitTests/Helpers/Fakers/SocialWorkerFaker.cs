using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class SocialWorkerFaker : Faker<SocialWorker>
{
    public SocialWorkerFaker()
    {
        RuleFor(a => a.RegistrationNumber, f => $"SW{f.Random.Number().ToString()}");
        RuleFor(a => a.RegisteredName, f => f.Name.FullName());
        RuleFor(a => a.Status, _ => "Registered");
        RuleFor(a => a.TownOfEmployment, f => f.Address.City());
        RuleFor(a => a.RegisteredFrom, f => f.Date.Recent());
        RuleFor(a => a.RegisteredUntil, f => f.Date.Soon());
        RuleFor(a => a.Registered, _ => true);
    }
}

public static class SocialWorkerFakerExtensions
{
    public static SocialWorker GenerateWithId(this SocialWorkerFaker accountFaker, int id)
    {
        return accountFaker.RuleFor(a => a.Id, _ => id.ToString()).Generate();
    }
}
