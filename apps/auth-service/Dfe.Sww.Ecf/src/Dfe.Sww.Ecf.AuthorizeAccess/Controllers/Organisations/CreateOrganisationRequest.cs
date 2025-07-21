using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Organisations;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;

[PublicAPI]
public record CreateOrganisationRequest
{
    public required string OrganisationName { get; set; }
    public Int64? ExternalOrganisationId { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
    public Guid? PrimaryCoordinatorId { get; set; }
    public string? Region { get; set; }

    public required CreatePersonRequest CreatePersonRequest { get; set; }
}

public static class CreateOrganisationRequestExtensions
{
    public static Organisation ToOrganisation(this CreateOrganisationRequest request) =>
        new()
        {
            OrganisationName = request.OrganisationName,
            ExternalOrganisationId = request.ExternalOrganisationId,
            LocalAuthorityCode = request.LocalAuthorityCode,
            Type = request.Type,
            PrimaryCoordinatorId = request.PrimaryCoordinatorId,
            Region = request.Region,
            PrimaryCoordinator = new()
            {
                FirstName = request.CreatePersonRequest.FirstName,
                LastName = request.CreatePersonRequest.LastName,
                MiddleName = request.CreatePersonRequest.MiddleName,
                EmailAddress = request.CreatePersonRequest.EmailAddress,
                PhoneNumber = request.CreatePersonRequest.PhoneNumber,
                Trn = request.CreatePersonRequest.SocialWorkEnglandNumber,
                PersonRoles = request.CreatePersonRequest
                    .Roles.Select(roleType => new PersonRole { RoleId = (int)roleType })
                    .ToList(),
                Status = request.CreatePersonRequest.Status,
                ExternalUserId = request.CreatePersonRequest.ExternalUserId,
                IsFunded = request.CreatePersonRequest.IsFunded,
                ProgrammeStartDate = request.CreatePersonRequest.ProgrammeStartDate,
                ProgrammeEndDate = request.CreatePersonRequest.ProgrammeEndDate
            }
        };
}
