using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Organisations;
using Hangfire.Annotations;

namespace Dfe.Sww.Ecf.Core.Services.Organisations;

[PublicAPI]
public class OrganisationDto
{
    public Guid OrganisationId { get; init; }
    public required string OrganisationName { get; set; }
    public Int64? ExternalOrganisationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
    public Guid? PrimaryCoordinatorId { get; set; }
}

public static class OrganisationDtoExtensions
{
    public static OrganisationDto ToDto(this Organisation organisation) =>
        new()
        {
            OrganisationId = organisation.OrganisationId,
            OrganisationName = organisation.OrganisationName,
            ExternalOrganisationId = organisation.ExternalOrganisationId,
            CreatedOn = organisation.CreatedOn,
            UpdatedOn = organisation.UpdatedOn,
            LocalAuthorityCode = organisation.LocalAuthorityCode,
            Type = organisation.Type,
            PrimaryCoordinatorId = organisation.PrimaryCoordinatorId
        };
}
