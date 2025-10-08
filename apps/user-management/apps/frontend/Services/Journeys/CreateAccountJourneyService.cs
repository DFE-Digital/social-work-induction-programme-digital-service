using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class CreateAccountJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IAccountService accountService,
    IOrganisationService organisationService,
    EcfLinkGenerator linkGenerator,
    IEmailService emailService
) : ICreateAccountJourneyService
{
    private const string CreateAccountSessionKey = "_createAccount";
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

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
        if (createAccountJourneyModel.AccountDetails is not null && isStaff is not null) createAccountJourneyModel.AccountDetails.IsStaff = (bool)isStaff;

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

    public AccountChangeLinks GetAccountChangeLinks(Guid? organisationId = null)
    {
        return new AccountChangeLinks
        {
            UserTypeChangeLink = linkGenerator.ManageAccount.SelectAccountTypeChange(organisationId),
            AccountTypesChangeLink = linkGenerator.ManageAccount.SelectUseCaseChange(organisationId),
            RegisteredWithSocialWorkEnglandChangeLink = linkGenerator.ManageAccount.EligibilitySocialWorkEnglandChange(organisationId),
            StatutoryWorkerChangeLink = linkGenerator.ManageAccount.EligibilityStatutoryWorkChange(organisationId),
            AgencyWorkerChangeLink = linkGenerator.ManageAccount.EligibilityAgencyWorkerChange(organisationId),
            RecentlyQualifiedChangeLink = linkGenerator.ManageAccount.EligibilityQualificationChange(organisationId),
            FirstNameChangeLink = linkGenerator.ManageAccount.AddAccountDetailsChangeFirstName(organisationId),
            MiddleNamesChangeLink = linkGenerator.ManageAccount.AddAccountDetailsChangeMiddleNames(organisationId),
            LastNameChangeLink = linkGenerator.ManageAccount.AddAccountDetailsChangeLastName(organisationId),
            EmailChangeLink = linkGenerator.ManageAccount.AddAccountDetailsChangeEmail(organisationId),
            SocialWorkEnglandNumberChangeLink = linkGenerator.ManageAccount.AddAccountDetailsChangeSocialWorkEnglandNumber(organisationId),
            ProgrammeDatesChangeLink = linkGenerator.ManageAccount.SocialWorkerProgrammeDates(organisationId)
        };
    }

    public void ResetCreateAccountJourneyModel()
    {
        Session.Remove(CreateAccountSessionKey);
    }

    public async Task<Account> CompleteJourneyAsync(Guid? organisationId = null)
    {
        var organisation = await organisationService.GetByIdAsync(organisationId);

        var createAccountJourneyModel = GetCreateAccountJourneyModel();

        var account = createAccountJourneyModel.ToAccount();

        account = await accountService.CreateAsync(account, organisationId);

        await emailService.SendInvitationEmailAsync(new InvitationEmailRequest
        {
            AccountId = account.Id,
            OrganisationName = organisation?.OrganisationName ?? string.Empty,
            Role = account.Types?.Min() // Get the highest ranking role - the lowest (int)enum
        });

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

    private CreateAccountJourneyModel GetCreateAccountJourneyModel()
    {
        Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );
        return createAccountJourneyModel ?? new CreateAccountJourneyModel();
    }

    private void SetCreateAccountJourneyModel(CreateAccountJourneyModel createAccountJourneyModel)
    {
        Session.Set(CreateAccountSessionKey, createAccountJourneyModel);
    }
}
