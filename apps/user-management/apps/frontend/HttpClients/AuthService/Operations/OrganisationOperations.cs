using System.Security.Claims;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

public class OrganisationOperations(AuthServiceClient authServiceClient)
    : IOrganisationOperations
{
    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };

    public async Task<PaginationResult<OrganisationDto>> GetAllAsync(PaginationRequest request)
    {
        var route = $"/api/Organisations?Offset={request.Offset}&PageSize={request.PageSize}";
        var httpResponse = await authServiceClient.HttpClient.GetAsync(route);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to get organisations.");
        }

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
}
