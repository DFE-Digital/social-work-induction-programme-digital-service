using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IEditOrganisationJourneyService
{
    Task<PrimaryCoordinatorChangeType?> GetPrimaryCoordinatorChangeTypeAsync(Guid organisationId);
    Task SetPrimaryCoordinatorChangeTypeAsync(Guid organisationId, PrimaryCoordinatorChangeType? localAuthorityCode);
    public Task<Organisation?> GetOrganisationAsync(Guid organisationId);
    Task SetOrganisationAsync(Guid organisationId, Organisation organisation);
    Task<AccountDetails?> GetPrimaryCoordinatorAccountAsync(Guid organisationId);
    Task SetPrimaryCoordinatorAccountAsync(Guid organisationId, AccountDetails account);
    void ResetEditOrganisationJourneyModel(Guid organisationId);
    Task<Organisation?> CompleteJourneyAsync(Guid organisationId);
}
