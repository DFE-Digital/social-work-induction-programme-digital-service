using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface IMoodleService
{
    Task<int?> CreateUserAsync(AccountDetails accountDetails);
    Task<int?> UpdateUserAsync(AccountDetails accountDetails);
    Task<int?> CreateCourseAsync(Organisation organisation);
    Task<bool> EnrolUserAsync(int externalUserId, int externalOrgId, MoodleRoles moodleRole);
}
