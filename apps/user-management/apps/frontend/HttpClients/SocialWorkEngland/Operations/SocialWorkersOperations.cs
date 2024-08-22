using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Operations;

public class SocialWorkersOperations : ISocialWorkersOperations
{
    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };

    // private readonly ILogger<SocialWorkOperations> _logger;
    private readonly SocialWorkEnglandClient _socialWorkEnglandClient;

    public SocialWorkersOperations(
        // ILogger<SocialWorkOperations> logger,
        SocialWorkEnglandClient socialWorkEnglandClient
    )
    {
        // _logger = logger;
        _socialWorkEnglandClient = socialWorkEnglandClient;
    }

    public async Task<SocialWorker?> GetById(int id)
    {
        // _logger.LogWarning("Attempting to retrieve Social Worker ID Number - {id}", id);

        var route =
            _socialWorkEnglandClient.Options.Routes?.SocialWorker?.GetById
            ?? "/GetSocialWorkerById";

        var httpResponse = await _socialWorkEnglandClient.HttpClient.GetAsync(
            route + $"?swid={id}"
        );

        var result = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            // TODO Error/Exception Handling?
            // _logger.LogWarning("Failed to retrieve Social Worker - {result}", result);
            return null;
        }

        // _logger.LogWarning("$Retrieved Social Worker - {result}", result);

        return JsonSerializer.Deserialize<SocialWorker>(result, SerializerOptions);
    }
}
