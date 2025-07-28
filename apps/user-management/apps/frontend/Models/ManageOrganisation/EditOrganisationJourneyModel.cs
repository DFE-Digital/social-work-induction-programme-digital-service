namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public class EditOrganisationJourneyModel(Organisation organisation, AccountDetails primaryCoordinatorAccount)
{
    public PrimaryCoordinatorChangeType? PrimaryCoordinatorChangeType { get; set; }
    public Organisation? Organisation { get; set; } = organisation;
    public AccountDetails? PrimaryCoordinatorAccount { get; set; } = primaryCoordinatorAccount;
}
