using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IEditOrganisationJourneyService
{
    PrimaryCoordinatorChangeType? GetPrimaryCoordinatorChangeType();
    void SetPrimaryCoordinatorChangeType(PrimaryCoordinatorChangeType? localAuthorityCode);
    void ResetEditOrganisationJourneyModel();
    Task<Organisation?> CompleteJourneyAsync();
}
