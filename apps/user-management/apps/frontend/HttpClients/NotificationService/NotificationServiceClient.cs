using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService;

public class NotificationServiceClient : INotificationServiceClient
{
    public NotificationServiceClient(
        HttpClient httpClient,
        IOptions<NotificationClientOptions> clientOptions
    )
    {
        HttpClient = httpClient;
        Options = clientOptions.Value;
        Notification = new NotificationOperations(this);
    }

    internal HttpClient HttpClient { get; init; }

    internal NotificationClientOptions Options { get; init; }

    public INotificationOperations Notification { get; init; }
}
