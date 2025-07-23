namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public class EditOrganisationJourneyModel
{
    public PrimaryCoordinatorChangeType? PrimaryCoordinatorChangeType { get; set; }
    public Organisation? Organisation { get; set; }
    public Account? PrimaryCoordinatorAccount { get; set; }
}
