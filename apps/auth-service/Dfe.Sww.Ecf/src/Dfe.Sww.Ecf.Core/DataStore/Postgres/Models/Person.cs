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

    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();
}
