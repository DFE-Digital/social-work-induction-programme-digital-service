using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

public class OrganisationOperations(AuthServiceClient authServiceClient)
    : BaseOperations, IOrganisationOperations
{
    public async Task<PaginationResult<OrganisationDto>> GetAllAsync(PaginationRequest request)
    {
        var route = $"/api/Organisations?Offset={request.Offset}&PageSize={request.PageSize}";
        var httpResponse = await authServiceClient.HttpClient.GetAsync(route);

        HandleHttpResponse(httpResponse, "Failed to get organisations.");

        var response = await httpResponse.Content.ReadAsStringAsync();

        var organisations = JsonSerializer.Deserialize<PaginationResult<OrganisationDto>>(
            response,
            SerializerOptions
        );

        if (organisations is null)
        {
            throw new InvalidOperationException("Failed to get organisations.");
        }

        return organisations;
    }

    public async Task<OrganisationDto> CreateAsync(CreateOrganisationRequest createOrganisationRequest)
    {
        var httpResponse = await authServiceClient.HttpClient.PostAsJsonAsync(
            "/api/Organisations/Create",
            createOrganisationRequest
        );

        HandleHttpResponse(httpResponse, "Failed to create organisation.");

        var response = await httpResponse.Content.ReadAsStringAsync();
        var createdOrganisation = JsonSerializer.Deserialize<OrganisationDto>(response, SerializerOptions);
        if (createdOrganisation is null)
        {
            throw new InvalidOperationException("Failed to create organisation.");
        }
        return createdOrganisation;
    }

    public async Task<OrganisationDto?> GetByIdAsync(Guid id)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync($"/api/Organisations/{id}");

        HandleHttpResponse(httpResponse, $"Failed to get organisation with ID {id}.");

        var response = await httpResponse.Content.ReadAsStringAsync();
        var organisation = JsonSerializer.Deserialize<OrganisationDto>(response, SerializerOptions);
        if (organisation is null)
        {
            throw new InvalidOperationException("Failed to get organisation.");
        }
        return organisation;
    }

    public async Task<OrganisationDto> GetByLocalAuthorityCodeAsync(int localAuthorityCode)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync($"/api/Organisations/{localAuthorityCode}");

        HandleHttpResponse(httpResponse, $"Failed to get organisation with local authority code {localAuthorityCode}.");

        var response = await httpResponse.Content.ReadAsStringAsync();
        var organisation = JsonSerializer.Deserialize<OrganisationDto>(response, SerializerOptions);
        if (organisation is null)
        {
            throw new InvalidOperationException("Failed to get local authority data.");
        }
        return organisation;
    }
}
