namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class Person
{
    public required Guid PersonId { get; init; }
    public required DateTime? CreatedOn { get; init; }
    public required DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public required string? Trn { get; set; }
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly? DateOfBirth { get; set; }
    public string? EmailAddress { get; set; }
    public string? NationalInsuranceNumber { get; set; }
}
