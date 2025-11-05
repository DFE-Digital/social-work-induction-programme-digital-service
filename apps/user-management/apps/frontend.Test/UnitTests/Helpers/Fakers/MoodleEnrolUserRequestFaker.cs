using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class MoodleEnrolUserRequestFaker : Faker<EnrolUserRequest>
{
    public MoodleEnrolUserRequestFaker()
    {
        RuleFor(a => a.RoleId, f => f.PickRandom<MoodleRoles>());
        RuleFor(a => a.UserId, f => f.Random.Int());
        RuleFor(a => a.CourseId, f => f.Random.Int());
    }
}
