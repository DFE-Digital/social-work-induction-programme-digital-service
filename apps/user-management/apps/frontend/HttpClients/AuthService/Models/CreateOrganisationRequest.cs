using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

[PublicAPI]
public record CreateOrganisationRequest
{
    public required string OrganisationName { get; set; }
    public int? ExternalOrganisationId { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
    public Guid? PrimaryCoordinatorId { get; set; }
    public string? Region { get; set; }

    public required CreatePersonRequest CreatePersonRequest { get; set; }
}
