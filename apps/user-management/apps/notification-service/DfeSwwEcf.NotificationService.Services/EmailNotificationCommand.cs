using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services.Interfaces;
using Notify.Interfaces;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace DfeSwwEcf.NotificationService.Services;

/// <inheritdoc/>
public class EmailNotificationCommand(IAsyncNotificationClient notificationClient) : INotificationCommand
{
    private readonly IAsyncNotificationClient _notificationClient = notificationClient;

    /// <inheritdoc/>
    public async Task SendNotificationAsync(NotificationRequest notificationRequest)
    {
        var delay = Backoff.ExponentialBackoff(TimeSpan.FromSeconds(1), retryCount: 5);
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(delay);

        await retryPolicy.ExecuteAsync(async () =>
            await _notificationClient.SendEmailAsync(
             emailAddress: notificationRequest.EmailAddress,
             templateId: notificationRequest.TemplateId.ToString(),
             personalisation: notificationRequest.Personalisation?.ToDictionary(kvp => kvp.Key, kvp => (dynamic)kvp.Value) ?? null,
             clientReference: notificationRequest?.Reference ?? null,
             emailReplyToId: notificationRequest?.EmailReplyToId?.ToString() ?? null
            )
        );
    }
}
