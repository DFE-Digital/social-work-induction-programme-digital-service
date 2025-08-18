using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services.Email;

public class EmailService(
    IAuthServiceClient authService,
    INotificationServiceClient notificationService,
    EcfLinkGenerator linkGenerator,
    IAccountService accountService,
    IOptions<EmailTemplateOptions> templateOptions,
    IHttpContextAccessor httpContextAccessor)
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

        // TODO: Refactor invite emails to be a singular template with role-based personalisation
        var templateId = request.Role.HasValue
            ? _templateOptions.Roles[request.Role.Value].Invitation
            : _templateOptions.PrimaryCoordinatorInvitationEmail;

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
}
