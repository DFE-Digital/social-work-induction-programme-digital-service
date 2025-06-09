namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class Person
{
    public Guid PersonId { get; init; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? Trn { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? EmailAddress { get; set; }
    public string? NationalInsuranceNumber { get; set; }
    public PersonStatus? Status { get; set; }
    public int? ExternalUserId { get; set; }
    public bool IsFunded { get; set; }

    /// <summary>
    /// A flag for if an ECSW account type has completed the registration flow
    /// </summary>
    /// <returns>true if the ECSW user has completed the registration flow</returns>
    /// <returns>false if the ECSW user has not completed the registration flow, or the default value when creating an ECSW</returns>
    /// <returns>null if the account type is not an ECSW</returns>
    public bool? CompletedEcswRegistration { get; set; }

    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();
    public ICollection<PersonOrganisation> PersonOrganisations { get; set; } = new List<PersonOrganisation>();
}
