using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Operations;

public class CourseOperations(MoodleServiceClient moodleServiceClient) : ICourseOperations
{
    private readonly MoodleServiceClient _moodleServiceClient = moodleServiceClient;

    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web);

    public async Task<CreateCourseResponse> CreateAsync(CreateCourseRequest request)
    {
        var parameters = new Dictionary<string, string>
        {
            { "wstoken", _moodleServiceClient.Options.ApiToken },
            { "wsfunction", FunctionNameConstants.CreateCourse },
            { "moodlewsrestformat", "json" },
            { "courses[0][fullname]", request.FullName },
            { "courses[0][shortname]", request.ShortName },
            { "courses[0][categoryid]", request.CategoryId.ToString() }
        };

        using var content = new FormUrlEncodedContent(parameters);

        var httpResponse = await _moodleServiceClient.HttpClient.PostAsync(string.Empty, content);

        if (!httpResponse.IsSuccessStatusCode)
        {
            return new CreateCourseResponse { Successful = false };
        }

        var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<IList<CreateCourseResponse>>(
            jsonResponse,
            SerializerOptions
        );

        return result?.FirstOrDefault() ?? new CreateCourseResponse { Successful = false };
    }

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
