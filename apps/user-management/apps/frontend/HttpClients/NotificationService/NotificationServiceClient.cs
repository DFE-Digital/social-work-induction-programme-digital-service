using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService;

public class NotificationServiceClient : INotificationServiceClient
{
    public NotificationServiceClient(
        HttpClient httpClient,
        IOptions<NotificationClientOptions> clientOptions,
        ILogger<NotificationServiceClient> logger,
        IOptions<FeatureFlags> featureFlags)
    {
        HttpClient = httpClient;
        Options = clientOptions.Value;
        Notification = new NotificationOperations(HttpClient, Options.Routes.Notification, logger, featureFlags.Value);

        if (!string.IsNullOrEmpty(Options.FunctionKey))
        {
            logger.LogDebug("Adding x-functions-key header to all requests");
            HttpClient.DefaultRequestHeaders.Add("x-functions-key", Options.FunctionKey);
        }
        else
        {
            logger.LogWarning("No FunctionKey provided - requests will be unauthenticated");
        }
    }

    private HttpClient HttpClient { get; }

    private NotificationClientOptions Options { get; }

    public INotificationOperations Notification { get; init; }
}
