using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

public class AsyeSocialWorkerOperations(AuthServiceClient authServiceClient)
    : BaseOperations, IAsyeSocialWorkerOperations
{
    public async Task<bool> ExistsAsync(string socialWorkerId)
    {
        var httpResponse = await authServiceClient.HttpClient.GetAsync($"/api/AsyeSocialWorker/{socialWorkerId}");

        HandleHttpResponse(httpResponse, $"Failed check for social worker ID {socialWorkerId}.");

        var response = await httpResponse.Content.ReadAsStringAsync();

        var person = JsonSerializer.Deserialize<bool>(response, SerializerOptions);

        return person;
    }
}
