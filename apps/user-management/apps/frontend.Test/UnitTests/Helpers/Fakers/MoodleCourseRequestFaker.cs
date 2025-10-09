using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class MoodleCourseRequestFaker : Faker<EnrolUserRequest>
{
    public MoodleCourseRequestFaker()
    {
        RuleFor(a => a.RoleId, f => f.PickRandom<MoodleRoles>());
        RuleFor(a => a.UserId, f => f.Random.Int());
        RuleFor(a => a.CourseId, f => f.Random.Int());
    }
}
