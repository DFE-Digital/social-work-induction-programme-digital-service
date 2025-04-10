using Bogus;
using DfeSwwEcf.SweApiSimulator.Models;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Helpers;

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
        RuleFor(a => a.Annotations, _ => []);
        RuleFor(a => a.Registered, _ => "True");
    }
}
