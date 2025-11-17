using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services.Email;

public class EmailService(
    IAuthServiceClient authService,
    INotificationServiceClient notificationService,
    EcfLinkGenerator linkGenerator,
    IAccountService accountService,
    IOptions<EmailTemplateOptions> templateOptions,
    IHttpContextAccessor httpContextAccessor,
    ILogger<EmailService> logger)
    : IEmailService
{
    private readonly EmailTemplateOptions _templateOptions = templateOptions.Value;

    public async Task SendInvitationEmailAsync(InvitationEmailRequest request)
    {
        if (httpContextAccessor.HttpContext == null) return;

        var account = await accountService.GetByIdAsync(request.AccountId);
        if (account?.Email == null) return;

        var linkingToken = await authService.Accounts.GetLinkingTokenByAccountIdAsync(request.AccountId);
        var invitationLink = linkGenerator.SignInWithLinkingToken(
            httpContextAccessor.HttpContext,
            linkingToken
        );

        var templateId = request.IsPrimaryCoordinator is true
            ? _templateOptions.PrimaryCoordinatorInvitationEmail
            : _templateOptions.Invitation;

        var notificationRequest = new NotificationRequest
        {
            EmailAddress = account.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", account.FullName },
                { "organisation", request.OrganisationName },
                { "invitation_link", invitationLink }
            }
        };

        await notificationService.Notification.SendEmailAsync(notificationRequest);
    }

    public async Task SendWelcomeEmailAsync(WelcomeEmailRequest request)
    {
        var account = await accountService.GetByIdAsync(request.AccountId);

        if (account?.Email == null)
        {
            logger.LogError("Email is required to send welcome email");
            return;
        }

        if (string.IsNullOrWhiteSpace(account.FullName))
        {
            logger.LogError("Full name is required to send welcome email");
            return;
        }

        var isEcsw = account.Types is not null && account.Types.Contains(AccountType.EarlyCareerSocialWorker) ? "yes" : "no";
        var isCoordinator = account.Types is not null && account.Types.Contains(AccountType.Coordinator) ? "yes" : "no";
        var isAssessor = account.Types is not null && account.Types.Contains(AccountType.Assessor) ? "yes" : "no";

        var templateId = _templateOptions.Welcome;

        var notificationRequest = new NotificationRequest
        {
            EmailAddress = account.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", account.FullName },
                { "ECSW", isEcsw },
                { "coordinator", isCoordinator },
                { "assessor", isAssessor }
            }
        };

        await notificationService.Notification.SendEmailAsync(notificationRequest);
    }
}
