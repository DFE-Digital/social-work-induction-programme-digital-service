using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class CreateOrganisationJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IOrganisationService organisationService,
    IAuthServiceClient authServiceClient,
    IAccountService accountService,
    INotificationServiceClient notificationServiceClient,
    IOptions<EmailTemplateOptions> emailTemplateOptions,
    EcfLinkGenerator linkGenerator
) : ICreateOrganisationJourneyService
{
    private const string CreateOrganisationSessionKey = "_createOrganisation";

    private ISession Session =>
        httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    public Organisation? GetOrganisation()
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        return createOrganisationJourneyModel.Organisation;
    }

    public void SetOrganisation(Organisation organisation)
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        createOrganisationJourneyModel.Organisation = organisation;
        SetCreateOrganisationJourneyModel(createOrganisationJourneyModel);
    }

    public int? GetLocalAuthorityCode()
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        return createOrganisationJourneyModel.LocalAuthorityCode;
    }

    public void SetLocalAuthorityCode(int? localAuthorityCode)
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        createOrganisationJourneyModel.LocalAuthorityCode = localAuthorityCode;
        SetCreateOrganisationJourneyModel(createOrganisationJourneyModel);
    }

    public AccountDetails? GetPrimaryCoordinatorAccountDetails()
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        return createOrganisationJourneyModel.PrimaryCoordinatorAccountDetails;
    }

    public void SetPrimaryCoordinatorAccountDetails(AccountDetails accountDetails)
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        createOrganisationJourneyModel.PrimaryCoordinatorAccountDetails = accountDetails;
        SetCreateOrganisationJourneyModel(createOrganisationJourneyModel);
    }

    public void ResetCreateOrganisationJourneyModel()
    {
        Session.Remove(CreateOrganisationSessionKey);
    }

    public async Task<Organisation?> CompleteJourneyAsync()
    {
        var createAccountJourneyModel = GetOrganisationJourneyModel();

        var organisation = createAccountJourneyModel.Organisation;
        var primaryCoordinator = createAccountJourneyModel.PrimaryCoordinatorAccountDetails;

        if (organisation is null || primaryCoordinator is null) throw new ArgumentNullException();

        // TODO implement call to Moodle for creating a person and organisation here, then set the ids
        organisation.ExternalOrganisationId = 123;
        primaryCoordinator.ExternalUserId = 123;

        var account = AccountDetails.ToAccount(primaryCoordinator);
        organisation = await organisationService.CreateAsync(organisation, account);

        ResetCreateOrganisationJourneyModel();

        if (organisation.PrimaryCoordinatorId is { } primaryCoordinatorId) await SendInvitationEmailAsync(primaryCoordinatorId, organisation.OrganisationName);

        return organisation;
    }

    private async Task SendInvitationEmailAsync(Guid accountId, string organisationName)
    {
        if (httpContextAccessor.HttpContext is null) return;

        var account = await accountService.GetByIdAsync(accountId);
        if (account?.Email is null) return;

        var linkingToken = await authServiceClient.Accounts.GetLinkingTokenByAccountIdAsync(
            accountId
        );

        var invitationLink = linkGenerator.SignInWithLinkingToken(
            httpContextAccessor.HttpContext,
            linkingToken
        );

        var templateId = emailTemplateOptions.Value.PrimaryCoordinatorInvitationEmail;
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = account.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", account.FullName },
                { "organisation", organisationName },
                { "invitation_link", invitationLink }
            }
        };

        await notificationServiceClient.Notification.SendEmailAsync(notificationRequest);
    }

    private CreateOrganisationJourneyModel GetOrganisationJourneyModel()
    {
        Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? createOrganisationJourneyModel
        );
        return createOrganisationJourneyModel ?? new CreateOrganisationJourneyModel();
    }

    private void SetCreateOrganisationJourneyModel(CreateOrganisationJourneyModel createOrganisationJourneyModel)
    {
        Session.Set(CreateOrganisationSessionKey, createOrganisationJourneyModel);
    }
}
