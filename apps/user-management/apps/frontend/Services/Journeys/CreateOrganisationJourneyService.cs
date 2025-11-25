using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
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
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        var organisation = createOrganisationJourneyModel.Organisation;
        var primaryCoordinator = createOrganisationJourneyModel.PrimaryCoordinatorAccountDetails;

        if (organisation is null || primaryCoordinator is null) throw new ArgumentNullException();

        var account = AccountDetails.ToAccount(primaryCoordinator);

        await CreateMoodleOrganisationAsync(organisation, account);

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

    private async Task CreateMoodleOrganisationAsync(Organisation organisation, Account primaryCoordinator)
    {
        if (featureFlags.Value.EnableMoodleIntegration)
        {
            var externalUserId = await moodleService.CreateUserAsync(primaryCoordinator);
            var externalOrgId = await moodleService.CreateCourseAsync(organisation);

            if (externalUserId is null || externalOrgId is null) throw new Exception(); // TODO handle unhappy path in separate ticket

            organisation.ExternalOrganisationId = externalOrgId.Value;
            primaryCoordinator.ExternalUserId = externalUserId.Value;

            await moodleService.EnrolUserAsync(externalUserId.Value, externalOrgId.Value, MoodleRoles.Manager);
        }
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
