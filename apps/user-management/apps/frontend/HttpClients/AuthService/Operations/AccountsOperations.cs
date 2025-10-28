using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

public class AccountsOperations(AuthServiceClient authServiceClient)
    : BaseOperations, IAccountsOperations
{
    public async Task<PaginationResult<Person>> GetAllAsync(PaginationRequest request, Guid? organisationId = null)
    {
        var organisationIdString = organisationId.HasValue
            ? organisationId.Value.ToString()
            : authServiceClient.HttpContextService.GetOrganisationId();

        var route = $"/api/Accounts?Offset={request.Offset}&PageSize={request.PageSize}&organisationId={organisationIdString}";
        var httpResponse = await authServiceClient.HttpClient.GetAsync(route);

        HandleHttpResponse(httpResponse, "Failed to get accounts.");

        var response = await httpResponse.Content.ReadAsStringAsync();

        var persons = DeserializeOrThrow<PaginationResult<Person>>(response, "Failed to get accounts.");

        return persons;
    }

    public async Task<Person?> GetByIdAsync(Guid id)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync($"/api/Accounts/{id}");

        HandleHttpResponse(httpResponse, $"Failed to get account with ID {id}.");

        var response = await httpResponse.Content.ReadAsStringAsync();

        var person = DeserializeOrThrow<Person>(response, $"Failed to get account with ID {id}.");

        return person;
    }

    public async Task<Person> CreateAsync(CreatePersonRequest createPersonRequest)
    {
        var httpResponse = await authServiceClient.HttpClient.PostAsJsonAsync(
            "/api/Accounts/Create",
            createPersonRequest
        );

        HandleHttpResponse(httpResponse, "Failed to create account.");

        var response = await httpResponse.Content.ReadAsStringAsync();
        var createdPerson = DeserializeOrThrow<Person>(response, "Failed to create account.");

        return createdPerson;
    }

    public async Task<string> GetLinkingTokenByAccountIdAsync(Guid accountId)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync(
            $"/api/Accounts/{accountId}/linking-token"
        );

        HandleHttpResponse(httpResponse, "Failed to get account linking token.");

        var response = await httpResponse.Content.ReadAsStringAsync();

        var linkingTokenResponse = DeserializeOrThrow<LinkingTokenResponse>(response, "Failed to get account linking token.");

        if (linkingTokenResponse?.LinkingToken is null)
        {
            throw new InvalidOperationException("Failed to get account linking token.");
        }

        return linkingTokenResponse.LinkingToken;
    }

    public async Task<Person> UpdateAsync(UpdatePersonRequest updatePersonRequest)
    {
        var httpResponse = await authServiceClient.HttpClient.PutAsJsonAsync(
            "/api/Accounts",
            updatePersonRequest
        );

        HandleHttpResponse(httpResponse, "Failed to update account.");

        var response = await httpResponse.Content.ReadAsStringAsync();

        var updatedPerson = DeserializeOrThrow<Person>(response, "Failed to update account.");

        return updatedPerson;
    }

    public async Task<bool> CheckEmailExistsAsync(CheckEmailRequest checkEmailRequest)
    {
        var httpResponse = await authServiceClient.HttpClient.PostAsJsonAsync("/api/Accounts/check", checkEmailRequest);

        HandleHttpResponse(httpResponse, $"Failed to check if email exists.");

        var response = await httpResponse.Content.ReadAsStringAsync();

        var exists = DeserializeOrThrow<bool>(response, "Failed to check if email exists.");

        return exists;
    }
}
