using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

[PublicAPI]
public record CreatePersonRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string EmailAddress { get; init; }

    // TODO: Rename this to SocialWorkEnglandNumber once the Auth service is updated
    public string? Trn { get; init; }
}
