using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EditOrganisationJourneyService(IHttpContextAccessor httpContextAccessor) : IEditOrganisationJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private const string EditOrganisationSessionKey = "_EditOrganisation";

    public void ResetCreateOrganisationJourneyModel()
    {
        Session.Remove(EditOrganisationSessionKey);
    }
}
