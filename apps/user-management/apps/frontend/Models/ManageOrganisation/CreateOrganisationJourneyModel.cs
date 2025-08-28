namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public class CreateOrganisationJourneyModel
{
    public Organisation? Organisation { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public string? PhoneNumber { get; set; }
    public AccountDetails? PrimaryCoordinatorAccountDetails { get; set; }
}
