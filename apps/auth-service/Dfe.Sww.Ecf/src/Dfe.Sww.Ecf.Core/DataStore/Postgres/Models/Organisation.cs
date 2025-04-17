namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class Organisation
{
    public const string ExternalOrganisationIdIndexName = "ix_organisation_external_organisation_id";

    public Guid OrganisationId { get; init; }
    public required string OrganisationName { get; set; }
    public required Int64 ExternalOrganisationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ICollection<PersonOrganisation> PersonOrganisations { get; set; } = new List<PersonOrganisation>();
}
