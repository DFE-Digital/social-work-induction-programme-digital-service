using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Organisations;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;

[PublicAPI]
public record CreateOrganisationRequest
{
    public required string OrganisationName { get; set; }
    public Int64? ExternalOrganisationId { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
    public Guid? PrimaryCoordinatorId { get; set; }
    public string? Region { get; set; }
}

public static class CreateOrganisationRequestExtensions
{
    public static Organisation ToOrganisation(this CreateOrganisationRequest request) =>
        new()
        {
            OrganisationName = request.OrganisationName,
            ExternalOrganisationId = request.ExternalOrganisationId,
            LocalAuthorityCode = request.LocalAuthorityCode,
            Type = request.Type,
            PrimaryCoordinatorId = request.PrimaryCoordinatorId,
            Region = request.Region
        };
}
