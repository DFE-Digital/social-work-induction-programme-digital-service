using Microsoft.Extensions.Options;
using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Interfaces;
using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Operations;
using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Options;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService;

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
