using System.Collections.Immutable;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;

[PublicAPI]
public record UpdatePersonRequest
{
    public required Guid PersonId { get; set; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string? EmailAddress { get; init; }
    public string? SocialWorkEnglandNumber { get; init; }
    public PersonStatus? Status { get; init; }
    public ImmutableList<RoleType> Roles { get; init; } = [];
    public bool? CompletedEcswRegistration { get; set; }
}

public static class UpdatePersonRequestExtensions
{
    public static Person ToPerson(this UpdatePersonRequest request) =>
        new()
        {
            PersonId = request.PersonId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailAddress = request.EmailAddress,
            Trn = request.SocialWorkEnglandNumber,
            PersonRoles = request
                .Roles.Select(roleType => new PersonRole { RoleId = (int)roleType })
                .ToList(),
            Status = request.Status,
            CompletedEcswRegistration = request.CompletedEcswRegistration
        };
}
