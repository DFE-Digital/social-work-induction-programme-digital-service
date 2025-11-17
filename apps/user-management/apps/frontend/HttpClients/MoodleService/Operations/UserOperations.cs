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

    public async Task<MoodleUserResponse> CreateAsync(MoodleUserRequest request)
    {
        return await SendMoodleRequestAsync(request, FunctionNameConstants.CreateUser);
    }

    public async Task<MoodleUserResponse> UpdateAsync(MoodleUserRequest request)
    {
        return await SendMoodleRequestAsync(request, FunctionNameConstants.UpdateUser);
    }

    private async Task<MoodleUserResponse> SendMoodleRequestAsync(MoodleUserRequest request, string wsFunction)
    {
        if (
            string.IsNullOrWhiteSpace(request.FirstName)
            || string.IsNullOrWhiteSpace(request.LastName)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Username)
        )
            return new MoodleUserResponse { Successful = false };

        var parameters = new Dictionary<string, string>
        {
            { "wstoken", _moodleServiceClient.Options.ApiToken },
            { "wsfunction", wsFunction },
            { "moodlewsrestformat", "json" },
            { "users[0][auth]", "oidc" },
            { "users[0][username]", request.Email.ToLower() },
            { "users[0][firstname]", request.FirstName },
            { "users[0][lastname]", request.LastName },
            { "users[0][email]", request.Email }
        };

        if (!string.IsNullOrWhiteSpace(request.MiddleName)) parameters.Add("users[0][middlename]", request.MiddleName);

        if (request.Id.HasValue
            && wsFunction == FunctionNameConstants.UpdateUser)
            parameters.Add("users[0][id]", request.Id.Value.ToString());

        using var content = new FormUrlEncodedContent(parameters);

        var httpResponse = await _moodleServiceClient.HttpClient.PostAsync(string.Empty, content);

        if (!httpResponse.IsSuccessStatusCode) return new MoodleUserResponse { Successful = false };

        var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

        if (wsFunction == FunctionNameConstants.UpdateUser) return new MoodleUserResponse { Id = request.Id, Username = request.Email.ToLower(), Successful = true };

        var result = JsonSerializer.Deserialize<IList<MoodleUserResponse>>(
            jsonResponse,
            SerializerOptions
        );
        return result?.FirstOrDefault() ?? new MoodleUserResponse { Successful = false };
    }
}
