namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class PersonOrganisation
{
    public const string PersonIdIndexName = "ix_person_organisation_person_id";
    public const string OrganisationIdIndexName = "ix_person_organisation_organisation_id";

    public Guid PersonOrganisationId { get; set; }
    public Guid PersonId { get; set; }
    public Guid OrganisationId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    // EF Navigation Properties
    public Person Person { get; set; } = null!;
    public Organisation Organisation { get; set; } = null!;
}
