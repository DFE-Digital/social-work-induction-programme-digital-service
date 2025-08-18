using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public class Organisation
{
    public Guid? OrganisationId { get; init; }

    [Display(Name = "Organisation name")] public string OrganisationName { get; init; } = "";

    public int? ExternalOrganisationId { get; set; }

    [Display(Name = "Local authority code")]
    public int? LocalAuthorityCode { get; init; }

    [Display(Name = "Type")] public OrganisationType? Type { get; init; }

    [Display(Name = "Local authority region")]
    public string? Region { get; init; }

    public Guid? PrimaryCoordinatorId { get; init; }
}
