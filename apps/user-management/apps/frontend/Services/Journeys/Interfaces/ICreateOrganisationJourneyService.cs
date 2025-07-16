using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface ICreateOrganisationJourneyService
{
    public Organisation? GetOrganisation();
    void SetOrganisation(Organisation organisation);
    int? GetLocalAuthorityCode();
    void SetLocalAuthorityCode(int? localAuthorityCode);
    void ResetCreateOrganisationJourneyModel();
}
