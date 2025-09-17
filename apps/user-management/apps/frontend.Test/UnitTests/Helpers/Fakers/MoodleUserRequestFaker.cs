using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class MoodleUserRequestFaker : Faker<MoodleUserRequest>
{
    public MoodleUserRequestFaker()
    {
        RuleFor(a => a.Id, f => f.Random.Int());
        RuleFor(a => a.Username, f => f.Internet.UserName());
        RuleFor(a => a.Email, f => f.Internet.Email());
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
    }
}
