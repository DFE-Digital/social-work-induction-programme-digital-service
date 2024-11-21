using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

public class AccountsOperations(AuthServiceClient authServiceClient) : IAccountsOperations
{
    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };

    public async Task<PaginationResult<Person>> GetAllAsync(PaginationRequest request)
    {
        var route = $"/api/Accounts?Offset={request.Offset}&PageSize={request.PageSize}";
        var httpResponse = await authServiceClient.HttpClient.GetAsync(route);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to get accounts.");
        }

        var response = await httpResponse.Content.ReadAsStringAsync();

        var persons = JsonSerializer.Deserialize<PaginationResult<Person>>(
            response,
            SerializerOptions
        );

        if (persons is null)
        {
            throw new InvalidOperationException("Failed to get accounts.");
        }

        return persons;
    }

    public async Task<Person> GetByIdAsync(Guid id)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync($"/api/Accounts/{id}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to get account.");
        }

        var response = await httpResponse.Content.ReadAsStringAsync();

        var person = JsonSerializer.Deserialize<Person>(response, SerializerOptions);

        if (person is null)
        {
            throw new InvalidOperationException("Failed to get account.");
        }

        return person;
    }

    public async Task<Person> CreateAsync(CreatePersonRequest createPersonRequest)
    {
        var httpResponse = await authServiceClient.HttpClient.PostAsJsonAsync(
            "/api/Accounts/Create",
            createPersonRequest
        );
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to create account.");
        }
        var response = await httpResponse.Content.ReadAsStringAsync();
        var createdPerson = JsonSerializer.Deserialize<Person>(response, SerializerOptions);
        if (createdPerson is null)
        {
            throw new InvalidOperationException("Failed to create account.");
        }
        return createdPerson;
    }

    public async Task<string> GetLinkingTokenByAccountIdAsync(Guid accountId)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync(
            $"/api/Accounts/{accountId}/linking-token"
        );

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to get account linking token.");
        }

        var response = await httpResponse.Content.ReadAsStringAsync();

        var linkingTokenResponse = JsonSerializer.Deserialize<LinkingTokenResponse>(
            response,
            SerializerOptions
        );

        if (linkingTokenResponse?.LinkingToken is null)
        {
            throw new InvalidOperationException("Failed to get account linking token.");
        }

        return linkingTokenResponse.LinkingToken;
    }
}
