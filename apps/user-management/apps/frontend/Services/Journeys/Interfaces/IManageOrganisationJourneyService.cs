using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IManageOrganisationJourneyService
{
    public Organisation? GetOrganisation();
    void SetOrganisation(Organisation organisation);
    int? GetLocalAuthorityCode();
    void SetLocalAuthorityCode(int? localAuthorityCode);
    AccountDetails? GetPrimaryCoordinatorAccountDetails();
    void SetPrimaryCoordinatorAccountDetails(AccountDetails accountDetails);
    void ResetOrganisationJourneyModel();
    Task<Organisation?> CompleteJourneyAsync();
}
