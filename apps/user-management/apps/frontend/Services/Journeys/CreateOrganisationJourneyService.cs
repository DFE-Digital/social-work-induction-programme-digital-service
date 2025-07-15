using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class CreateOrganisationJourneyService(IHttpContextAccessor httpContextAccessor) : ICreateOrganisationJourneyService
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

    public Organisation GetOrganisation()
    {
        var createOrganisationJourneyModel = GetOrganisationJourneyModel();
        return createOrganisationJourneyModel.ToOrganisation();
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

    public void ResetCreateOrganisationJourneyModel()
    {
        Session.Remove(CreateOrganisationSessionKey);
    }
}
