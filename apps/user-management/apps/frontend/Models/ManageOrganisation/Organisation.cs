using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

public class Organisation
{
    public Guid OrganisationId { get; init; }

    [Display(Name = "Organisation name")]
    public required string OrganisationName { get; set; }
    public required int ExternalOrganisationId { get; set; }

    [Display(Name = "LA code")]
    public int? LocalAuthorityCode { get; set; }

    [Display(Name = "Type")]
    public OrganisationType? Type { get; set; }
}
