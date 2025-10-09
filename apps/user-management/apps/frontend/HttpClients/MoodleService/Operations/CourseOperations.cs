using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Operations;

public class CourseOperations(MoodleServiceClient moodleServiceClient) : ICourseOperations
{
    private readonly MoodleServiceClient _moodleServiceClient = moodleServiceClient;

    public async Task<EnrolUserResponse> EnrolUserAsync(EnrolUserRequest request)
    {
        var parameters = new Dictionary<string, string>
        {
            { "wstoken", _moodleServiceClient.Options.ApiToken },
            { "wsfunction", FunctionNameConstants.EnrolUser },
            { "moodlewsrestformat", "json" },
            { "enrolments[0][roleid]", request.RoleId.ToString() },
            { "enrolments[0][userid]", request.UserId.ToString() },
            { "enrolments[0][courseid]", request.CourseId.ToString() }
        };

        using var content = new FormUrlEncodedContent(parameters);

        var httpResponse = await _moodleServiceClient.HttpClient.PostAsync(string.Empty, content);

        return new EnrolUserResponse { Successful = httpResponse.IsSuccessStatusCode };
    }
}
