using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Email;

public class InvitationEmailRequest
{
    public Guid AccountId { get; init; }
    public required string OrganisationName { get; init; }
    public bool? IsPrimaryCoordinator { get; init; }
}
