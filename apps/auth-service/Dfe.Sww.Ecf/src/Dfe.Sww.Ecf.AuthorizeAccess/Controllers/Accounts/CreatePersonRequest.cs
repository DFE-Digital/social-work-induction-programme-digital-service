using System.Collections.Immutable;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;

[PublicAPI]
public record CreatePersonRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string? EmailAddress { get; init; }

    public string? SocialWorkEnglandNumber { get; init; }

    public ImmutableList<RoleType> Roles { get; init; } = [];
}
