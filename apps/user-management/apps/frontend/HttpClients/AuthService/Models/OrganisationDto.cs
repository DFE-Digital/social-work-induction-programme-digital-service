using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

public class OrganisationDto
{
    public Guid OrganisationId { get; init; }
    public string? OrganisationName { get; set; }
    public int? ExternalOrganisationId { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
}
