using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

[PublicAPI]
public record UpdatePersonRequest
{
    public required Guid PersonId { get; set; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string EmailAddress { get; init; }
    public string? SocialWorkEnglandNumber { get; init; }
    public AccountStatus? Status { get; init; }
    public ImmutableList<AccountType> Roles { get; init; } = [];
}
