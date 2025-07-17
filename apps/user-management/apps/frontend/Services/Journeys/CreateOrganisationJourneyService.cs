using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class CreateOrganisationJourneyService(IHttpContextAccessor httpContextAccessor, IOrganisationService organisationService) : ICreateOrganisationJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private const string CreateOrganisationSessionKey = "_createOrganisation";

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
        return createOrganisationJourneyModel?.LocalAuthorityCode;
    }

    public void SetLocalAuthorityCode(int? localAuthorityCode)
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        createOrganisationJourneyModel.LocalAuthorityCode = localAuthorityCode;
        SetCreateOrganisationJourneyModel(createOrganisationJourneyModel);
    }

    public AccountDetails? GetPrimaryCoordinator()
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        // return createOrganisationJourneyModel.PrimaryCoordinator;
        return new AccountDetails();
    }

    public void SetPrimaryCoordinator(AccountDetails? primaryCoordinator)
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        // createOrganisationJourneyModel.PrimaryCoordinator = primaryCoordinator;
        SetCreateOrganisationJourneyModel(createOrganisationJourneyModel);
    }

    public void ResetCreateOrganisationJourneyModel()
    {
        Session.Remove(CreateOrganisationSessionKey);
    }

    public async Task<Organisation> CompleteJourneyAsync()
    {
        var createAccountJourneyModel = GetOrganisationJourneyModel();

        var organisation = createAccountJourneyModel.ToOrganisation();

        organisation = await organisationService.CreateAsync(organisation);

        // await SendInvitationEmailAsync(account);

        ResetCreateOrganisationJourneyModel();

        return organisation;
    }
}
