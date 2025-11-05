namespace Dfe.Sww.Ecf.Frontend.Services.Email.Models;

public class InvitationEmailRequest
{
    public Guid AccountId { get; init; }
    public required string OrganisationName { get; init; }
    public bool? IsPrimaryCoordinator { get; init; }
}
