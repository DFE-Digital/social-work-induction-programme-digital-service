namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public class CreateOrganisationJourneyModel
{
    public Organisation? Organisation { get; set; }
    public int? LocalAuthorityCode { get; set; }

    public AccountDetails? PrimaryCoordinatorAccountDetails { get; set; }

    public Organisation ToOrganisation()
    {
        return new Organisation
        {
            OrganisationName = Organisation?.OrganisationName,
            Type = Organisation?.Type,
            Region = Organisation?.Region,
            LocalAuthorityCode = LocalAuthorityCode
        };
    }
}
