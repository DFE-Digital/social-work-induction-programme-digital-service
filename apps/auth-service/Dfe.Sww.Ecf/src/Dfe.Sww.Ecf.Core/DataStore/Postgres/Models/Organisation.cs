using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Organisations;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class Organisation
{
    public const string ExternalOrganisationIdIndexName = "ix_organisation_external_organisation_id";

    public Guid OrganisationId { get; init; }
    public required string OrganisationName { get; set; }
    public int? ExternalOrganisationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
    public Guid? PrimaryCoordinatorId { get; set; }

    // EF Navigation Properties
    public ICollection<PersonOrganisation> PersonOrganisations { get; set; } = new List<PersonOrganisation>();
    public Person? PrimaryCoordinator { get; set; }
}
