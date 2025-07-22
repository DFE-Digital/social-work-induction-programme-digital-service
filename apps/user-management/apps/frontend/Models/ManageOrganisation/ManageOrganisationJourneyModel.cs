namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public class ManageOrganisationJourneyModel
{
    public Organisation? Organisation { get; set; }
    public int? LocalAuthorityCode { get; set; }

    public AccountDetails? PrimaryCoordinatorAccountDetails { get; set; }
}
