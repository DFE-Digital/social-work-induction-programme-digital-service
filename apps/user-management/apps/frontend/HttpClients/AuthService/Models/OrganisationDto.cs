using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

public class OrganisationDto
{
    public Guid OrganisationId { get; init; }
    public required string OrganisationName { get; set; }
    public required int ExternalOrganisationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
}
