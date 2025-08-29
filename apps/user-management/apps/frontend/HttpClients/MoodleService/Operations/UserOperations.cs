using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Operations;

public class UserOperations(MoodleServiceClient moodleServiceClient) : IUserOperations
{
    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web);

    private readonly MoodleServiceClient _moodleServiceClient = moodleServiceClient;

    public async Task<CreateMoodleUserResponse> CreateUserAsync(CreateMoodleUserRequest request)
    {
        if (
            string.IsNullOrWhiteSpace(request.FirstName)
            || string.IsNullOrWhiteSpace(request.LastName)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Username)
        )
        {
            return new CreateMoodleUserResponse { Successful = false };
        }

        var parameters = new Dictionary<string, string>
        {
            { "wstoken", _moodleServiceClient.Options.ApiToken },
            { "wsfunction", FunctionNameConstants.CreateUser },
            { "moodlewsrestformat", "json" },
            { "users[0][auth]", "oidc" },
            { "users[0][username]", request.Email.ToLower() },
            { "users[0][firstname]", request.FirstName },
            { "users[0][lastname]", request.LastName },
            { "users[0][email]", request.Email }
        };

        var content = new FormUrlEncodedContent(parameters);

        var httpResponse = await _moodleServiceClient.HttpClient.PostAsync(string.Empty, content);

        if (!httpResponse.IsSuccessStatusCode)
        {
            return new CreateMoodleUserResponse { Successful = false };
        }

        var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IList<CreateMoodleUserResponse>>(
            jsonResponse,
            SerializerOptions
        );
        return result?.FirstOrDefault() ?? new CreateMoodleUserResponse { Successful = false };
    }
}
