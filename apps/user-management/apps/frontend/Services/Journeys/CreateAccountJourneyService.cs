using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class CreateAccountJourneyService(
    IHttpContextAccessor httpContextAccessor,
    INotificationServiceClient notificationServiceClient,
    IOptions<EmailTemplateOptions> emailTemplateOptions,
    IAuthServiceClient authServiceClient,
    IAccountService accountService,
    IMoodleServiceClient moodleServiceClient,
    EcfLinkGenerator linkGenerator
) : ICreateAccountJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private const string CreateAccountSessionKey = "_createAccount";

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private CreateAccountJourneyModel GetCreateAccountJourneyModel()
    {
        Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );
        return createAccountJourneyModel ?? new CreateAccountJourneyModel();
    }

    public IList<AccountType>? GetAccountTypes()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.AccountTypes;
    }

    public AccountDetails? GetAccountDetails()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.AccountDetails;
    }

    public bool? GetIsStaff()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.IsStaff;
    }

    public void SetAccountDetails(AccountDetails accountDetails)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.AccountDetails = accountDetails;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public void SetAccountTypes(IList<AccountType> accountTypes)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.AccountTypes = accountTypes.ToImmutableList();
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public void SetIsStaff(bool? isStaff)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.IsStaff = isStaff;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public void SetExternalUserId(int? externalUserId)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.ExternalUserId = externalUserId;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public bool? GetIsRegisteredWithSocialWorkEngland()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.IsRegisteredWithSocialWorkEngland;
    }

    public void SetIsRegisteredWithSocialWorkEngland(bool? isRegisteredWithSocialWorkEngland)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.IsRegisteredWithSocialWorkEngland = isRegisteredWithSocialWorkEngland;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public bool? GetIsStatutoryWorker()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.IsStatutoryWorker;
    }

    public void SetIsStatutoryWorker(bool? isStatutoryWorker)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.IsStatutoryWorker = isStatutoryWorker;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public bool? GetIsAgencyWorker()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.IsAgencyWorker;
    }

    public void SetIsAgencyWorker(bool? isAgencyWorker)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.IsAgencyWorker = isAgencyWorker;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    /// <inheritdoc />
    public bool? GetIsRecentlyQualified()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.IsRecentlyQualified;
    }

    public void SetIsRecentlyQualified(bool? isRecentlyQualified)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.IsRecentlyQualified = isRecentlyQualified;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public DateOnly? GetProgrammeStartDate()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.ProgrammeStartDate;
    }

    public void SetProgrammeStartDate(DateOnly programmeStartDate)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.ProgrammeStartDate = programmeStartDate;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public DateOnly? GetProgrammeEndDate()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.ProgrammeEndDate;
    }

    public void SetProgrammeEndDate(DateOnly programmeEndDate)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.ProgrammeEndDate = programmeEndDate;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public AccountLabels GetAccountLabels()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        var accountLabels = new AccountLabels
        {
            IsStaffLabel =
                createAccountJourneyModel.IsStaff == true
                    ? IsStaffLabels.IsStaffTrue
                    : IsStaffLabels.IsStaffFalse,
            IsRegisteredWithSocialWorkEnglandLabel =
                createAccountJourneyModel.IsRegisteredWithSocialWorkEngland == true ? "Yes" : null,
            IsAgencyWorkerLabel = createAccountJourneyModel.IsAgencyWorker switch
            {
                true => "Yes",
                false => "No",
                null => null
            },
            IsStatutoryWorkerLabel =
                createAccountJourneyModel.IsStatutoryWorker == true ? "Yes" : null,
            IsRecentlyQualifiedLabel = createAccountJourneyModel.IsRecentlyQualified switch
            {
                true => "Yes",
                false => "No",
                null => null
            }
        };
        return accountLabels;
    }

    public AccountChangeLinks GetAccountChangeLinks()
    {
        return new AccountChangeLinks
        {
            SelectedAccountChangeLink = linkGenerator.SelectAccountType(),
            RegisteredWithSocialWorkEnglandChangeLink = linkGenerator.EligibilitySocialWorkEngland(),
            StatutoryWorkerChangeLink = linkGenerator.EligibilityStatutoryWork(),
            AgencyWorkerChangeLink = linkGenerator.EligibilityAgencyWorker(),
            RecentlyQualifiedChangeLink = linkGenerator.EligibilityQualification(),
            CoreDetailsChangeLink = linkGenerator.AddAccountDetailsChange(),
            ProgrammeDatesChangeLink = linkGenerator.SocialWorkerProgrammeDates()
        };
    }

    public void ResetCreateAccountJourneyModel()
    {
        Session.Remove(CreateAccountSessionKey);
    }

    private void SetCreateAccountJourneyModel(CreateAccountJourneyModel createAccountJourneyModel)
    {
        Session.Set(CreateAccountSessionKey, createAccountJourneyModel);
    }

    public async Task<Account> CompleteJourneyAsync()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();

        var account = createAccountJourneyModel.ToAccount();

        account = await accountService.CreateAsync(account);

        await SendInvitationEmailAsync(account);

        ResetCreateAccountJourneyModel();

        return account;
    }

    public void SetSocialWorkerDetails(SocialWorker socialWorkerDetails)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.SocialWorkerDetails = socialWorkerDetails;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public SocialWorker? GetSocialWorkerDetails()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.SocialWorkerDetails;
    }

    public async Task SendInvitationEmailAsync(Account account)
    {
        var accountTypes = account.Types;

        if (
            accountTypes is null
            || string.IsNullOrWhiteSpace(account.Email)
            || _httpContextAccessor.HttpContext is null
        )
        {
            return;
        }

        var linkingToken = await authServiceClient.Accounts.GetLinkingTokenByAccountIdAsync(
            account.Id
        );

        var invitationLink = linkGenerator.SignInWithLinkingToken(
            _httpContextAccessor.HttpContext,
            linkingToken
        );

        // Get the highest ranking role - the lowest (int)enum
        var invitationEmailType = accountTypes.Min();

        var templateId = emailTemplateOptions
            .Value
            .Roles[invitationEmailType.ToString()]
            .Invitation;
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = account.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", account.FullName },
                { "organisation", "TEST ORGANISATION" }, // TODO Retrieve this value when we can
                { "invitation_link", invitationLink }
            }
        };

        await notificationServiceClient.Notification.SendEmailAsync(notificationRequest);
    }
}
