namespace Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Models;

public class Person
{
    public required Guid PersonId { get; init; }
    public required DateTime CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public required string? Trn { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? EmailAddress { get; set; }
}
