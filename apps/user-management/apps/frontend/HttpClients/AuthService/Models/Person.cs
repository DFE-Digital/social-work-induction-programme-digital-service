using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

public class Person
{
    public required Guid PersonId { get; init; }
    public required DateTime CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? SocialWorkEnglandNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? EmailAddress { get; set; }
    public AccountStatus? Status { get; set; }
    public ImmutableList<AccountType> Roles { get; set; } = [];
    public bool IsFunded { get; set; }
}
