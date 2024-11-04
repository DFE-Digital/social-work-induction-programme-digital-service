using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Operations;

public class AccountsOperations(AuthServiceClient authServiceClient) : IAccountsOperations
{
    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };

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
}
