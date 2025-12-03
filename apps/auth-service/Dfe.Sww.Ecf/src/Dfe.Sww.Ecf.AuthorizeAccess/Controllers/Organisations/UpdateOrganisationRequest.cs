using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Organisations;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;

[PublicAPI]
public record UpdateOrganisationRequest
{
    public required Guid OrganisationId { get; set; }
    public required string OrganisationName { get; set; }
    public Int64? ExternalOrganisationId { get; set; }
    public int? LocalAuthorityCode { get; set; }
    public OrganisationType? Type { get; set; }
    public string? Region { get; set; }
    public string? PhoneNumber { get; init; }
}

public static class UpdateOrganisationRequestExtensions
{
    public static Organisation ToOrganisation(this UpdateOrganisationRequest request) =>
        new()
        {
            OrganisationId = request.OrganisationId,
            OrganisationName = request.OrganisationName,
            ExternalOrganisationId = request.ExternalOrganisationId,
            LocalAuthorityCode = request.LocalAuthorityCode,
            Type = request.Type,
            Region = request.Region,
            PhoneNumber = request.PhoneNumber,
        };
}
