using System.Collections.Immutable;
using SocialWorkInductionProgramme.Frontend.Extensions;
using Microsoft.Extensions.Options;
using SocialWorkInductionProgramme.Frontend.Configuration.Notification;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Interfaces;
using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Interfaces;
using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Models;
using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Models;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Routing;
using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.Journeys.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Services.Journeys;

public class CreateAccountJourneyService(
    IHttpContextAccessor httpContextAccessor,
    INotificationServiceClient notificationServiceClient,
    IOptions<EmailTemplateOptions> emailTemplateOptions,
    IAuthServiceClient authServiceClient,
    IAccountService accountService,
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

    private async Task SendInvitationEmailAsync(Account account)
    {
        var accountTypes = GetAccountTypes();

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
