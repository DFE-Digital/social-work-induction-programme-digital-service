using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class MoodleService(
    IMoodleServiceClient moodleServiceClient
) : IMoodleService
{
    public async Task<int?> CreateUserAsync(AccountDetails accountDetails)
    {
        var moodleRequest = new MoodleUserRequest
        {
            Username = accountDetails.Email,
            Email = accountDetails.Email,
            FirstName = accountDetails.FirstName,
            MiddleName = accountDetails.MiddleNames,
            LastName = accountDetails.LastName
        };

        var response = await moodleServiceClient.User.CreateAsync(moodleRequest);
        if (!response.Successful)
            throw new Exception($"Failed to create Moodle user with email {moodleRequest.Email}"); // TODO handle unhappy path in separate ticket

        return response.Id;
    }

    public async Task<int?> UpdateUserAsync(AccountDetails accountDetails)
    {
        var moodleRequest = new MoodleUserRequest
        {
            Id = accountDetails.ExternalUserId,
            Username = accountDetails.Email,
            Email = accountDetails.Email,
            FirstName = accountDetails.FirstName,
            MiddleName = accountDetails.MiddleNames,
            LastName = accountDetails.LastName
        };

        var response = await moodleServiceClient.User.UpdateAsync(moodleRequest);
        if (!response.Successful)
            throw new Exception($"Failed to update Moodle user with email {moodleRequest.Email}"); // TODO handle unhappy path in separate ticket

        return response.Id;
    }

    public async Task<int?> CreateCourseAsync(Organisation organisation)
    {
        var moodleRequest = new CreateCourseRequest
        {
            // TODO update this mapping when decision has been made - assumptions for now
            FullName = organisation.OrganisationName,
            ShortName = organisation.OrganisationName,
            CategoryId = 1
        };

        var response = await moodleServiceClient.Course.CreateAsync(moodleRequest);
        if (!response.Successful)
            throw new Exception($"Failed to create Moodle course with name {organisation.OrganisationName}"); // TODO handle unhappy path in separate ticket

        return response.Id;
    }

    public async Task<bool> EnrolUserAsync(int externalUserId, int externalOrgId, MoodleRoles moodleRole)
    {
        var enrolUserRequest = new EnrolUserRequest
        {
            UserId = externalUserId,
            CourseId = externalOrgId,
            RoleId = moodleRole
        };

        var response = await moodleServiceClient.Course.EnrolUserAsync(enrolUserRequest);
        if (!response.Successful)
            throw new Exception($"Failed to enrol user (ID {externalUserId}) onto Course (ID {externalOrgId}"); // TODO handle unhappy path in separate ticket

        return response.Successful;
    }
}
