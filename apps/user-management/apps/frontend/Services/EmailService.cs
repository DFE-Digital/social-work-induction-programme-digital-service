using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class EmailService(
    INotificationServiceClient notificationServiceClient,
    IOptions<EmailTemplateOptions> emailTemplateOptions
) : IEmailService
{
    public async Task<bool> PauseAccountAsync(
        AccountDetails? accountDetails,
        ImmutableList<AccountType>? accountTypes,
        string? coordinatorName,
        string? coordinatorEmail
    )
    {
        if (
            accountTypes is null
            || string.IsNullOrWhiteSpace(accountDetails?.Email)
            || string.IsNullOrWhiteSpace(coordinatorEmail)
            || string.IsNullOrWhiteSpace(coordinatorName)
        )
        {
            return false;
        }

        // Get the highest ranking role - the lowest (int)enum
        var invitationEmailType = accountTypes.Min();

        var templateId = emailTemplateOptions.Value.Roles[invitationEmailType.ToString()].Pause;

        var notificationRequest = new NotificationRequest
        {
            EmailAddress = accountDetails.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", accountDetails.FullName },
                { "organisation", "TEST ORGANISATION" }, // TODO Retrieve this value when we can
                { "coordinator name", coordinatorName },
                { "coordinator email", coordinatorEmail }
            }
        };

        var response = await notificationServiceClient.Notification.SendEmailAsync(
            notificationRequest
        );

        return IsSuccessStatusCode((int)response.StatusCode);
    }

    public async Task<bool> UnpauseAccountAsync(
        AccountDetails? accountDetails,
        ImmutableList<AccountType>? accountTypes,
        string? coordinatorName,
        string? coordinatorEmail
    )
    {
        if (
            accountTypes is null
            || string.IsNullOrWhiteSpace(accountDetails?.Email)
            || string.IsNullOrWhiteSpace(coordinatorEmail)
            || string.IsNullOrWhiteSpace(coordinatorName)
        )
        {
            return false;
        }

        // Get the highest ranking role - the lowest (int)enum
        var invitationEmailType = accountTypes.Min();

        var templateId = emailTemplateOptions.Value.Roles[invitationEmailType.ToString()].Unpause;

        var notificationRequest = new NotificationRequest
        {
            EmailAddress = accountDetails.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", accountDetails.FullName },
                { "organisation", "TEST ORGANISATION" }, // TODO Retrieve this value when we can
                { "coordinator name", coordinatorName },
                { "coordinator email", coordinatorEmail }
            }
        };

        var response = await notificationServiceClient.Notification.SendEmailAsync(
            notificationRequest
        );

        return IsSuccessStatusCode((int)response.StatusCode);
    }

    private static bool IsSuccessStatusCode(int statusCode)
    {
        return statusCode is >= 200 and <= 299;
    }
}
