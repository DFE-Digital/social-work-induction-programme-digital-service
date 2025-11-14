using System.Net;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

public class LocalAuthorityOperations(AuthServiceClient authServiceClient)
    : BaseOperations, ILocalAuthorityOperations
{
    public async Task<Organisation?> GetByLocalAuthorityCodeAsync(int localAuthorityCode)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync($"/api/LocalAuthority/{localAuthorityCode}");

        if (httpResponse.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }

        HandleHttpResponse(httpResponse, $"Failed to get organisation with local authority code {localAuthorityCode}.");

        var response = await httpResponse.Content.ReadAsStringAsync();
        var localAuthority = JsonSerializer.Deserialize<LocalAuthorityDto>(response, SerializerOptions);
        if (localAuthority is null)
        {
            throw new InvalidOperationException("Failed to get local authority data.");
        }
        return localAuthority.ToOrganisation();
    }
}
