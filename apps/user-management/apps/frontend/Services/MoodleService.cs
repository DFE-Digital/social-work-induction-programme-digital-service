using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class MoodleService(
    IMoodleServiceClient moodleServiceClient
) : IMoodleService
{
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
}
