using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public enum PrimaryCoordinatorChangeType
{
    [Display(Name = "Updating the existing coordinator's details")] UpdateExistingCoordinator,
    [Display(Name = "Replacing them with someone else")] ReplaceWithNewCoordinator
}
