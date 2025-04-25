using System.Collections.Immutable;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Hangfire.Annotations;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

[PublicAPI]
public class PersonDto
{
    public Guid PersonId { get; init; }
    public DateTime? CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; init; }
    public string? SocialWorkEnglandNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? EmailAddress { get; set; }
    public ImmutableList<RoleType> Roles { get; set; } = [];
    public PersonStatus? Status { get; set; }
    public int? ExternalUserId { get; set; }
}

public static class PersonDtoExtensions
{
    public static PersonDto ToDto(this Person person) =>
        new()
        {
            PersonId = person.PersonId,
            CreatedOn = person.CreatedOn,
            UpdatedOn = person.UpdatedOn,
            SocialWorkEnglandNumber = person.Trn,
            FirstName = person.FirstName,
            LastName = person.LastName,
            EmailAddress = person.EmailAddress,
            Roles = person.PersonRoles.Select(x => x.Role.RoleName).ToImmutableList() ?? [],
            Status = person.Status,
            ExternalUserId = person.ExternalUserId
        };
}
