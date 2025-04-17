namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;

public sealed class OidcConfiguration
{
    public const string ConfigurationKey = "Oidc";

    public string? Issuer { get; init; }

    public OidcApplicationConfiguration[] Applications { get; init; } = [];
}
