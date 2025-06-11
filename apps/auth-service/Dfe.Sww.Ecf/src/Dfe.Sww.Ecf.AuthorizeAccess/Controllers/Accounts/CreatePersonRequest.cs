using System.Collections.Immutable;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;

[PublicAPI]
public record CreatePersonRequest
{
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string? EmailAddress { get; init; }
    public string? SocialWorkEnglandNumber { get; init; }
    public PersonStatus? Status { get; init; }
    public ImmutableList<RoleType> Roles { get; init; } = [];
    public Guid OrganisationId { get; init; }
    public int? ExternalUserId { get; set; }
    public bool IsFunded { get; set; }
    public DateOnly? ProgrammeStartDate { get; init; }
    public DateOnly? ProgrammeEndDate { get; init; }
}

public static class CreatePersonRequestExtensions
{
    public static Person ToPerson(this CreatePersonRequest request) =>
        new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            EmailAddress = request.EmailAddress,
            Trn = request.SocialWorkEnglandNumber,
            PersonRoles = request
                .Roles.Select(roleType => new PersonRole { RoleId = (int)roleType })
                .ToList(),
            Status = request.Status,
            PersonOrganisations = new List<PersonOrganisation>
            {
                new() { OrganisationId = request.OrganisationId }
            },
            ExternalUserId = request.ExternalUserId,
            IsFunded = request.IsFunded,
            ProgrammeStartDate = request.ProgrammeStartDate,
            ProgrammeEndDate = request.ProgrammeEndDate
        };
}
