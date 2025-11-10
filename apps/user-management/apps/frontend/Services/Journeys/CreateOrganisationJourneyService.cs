using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class CreateOrganisationJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IOrganisationService organisationService,
    IEmailService emailService,
    IMoodleService moodleService,
    IOptions<FeatureFlags> featureFlags
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

    public string? GetPhoneNumber()
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        return createOrganisationJourneyModel.PhoneNumber;
    }

    public void SetPhoneNumber(string? phoneNumber)
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        createOrganisationJourneyModel.PhoneNumber = phoneNumber;
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

        // TODO implement call to Moodle for creating a person here, then set the id
        if (featureFlags.Value.EnableMoodleIntegration)
        {
            organisation.ExternalOrganisationId = await moodleService.CreateCourseAsync(organisation);
            primaryCoordinator.ExternalUserId = 123;
        }

        var account = AccountDetails.ToAccount(primaryCoordinator);
        organisation = await organisationService.CreateAsync(organisation, account);

        ResetCreateOrganisationJourneyModel();

        if (organisation.PrimaryCoordinatorId is { } primaryCoordinatorId)
            await emailService.SendInvitationEmailAsync(new InvitationEmailRequest
            {
                AccountId = primaryCoordinatorId,
                OrganisationName = organisation.OrganisationName,
                IsPrimaryCoordinator = true
            });

        return organisation;
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
