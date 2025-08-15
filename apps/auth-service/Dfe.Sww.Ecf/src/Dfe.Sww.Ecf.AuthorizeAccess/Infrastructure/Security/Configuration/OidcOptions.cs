namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;

public sealed class OidcOptions
{
    public const string ConfigurationKey = "Oidc";

    public required string Issuer { get; init; }

    public string? SigningCertificateName { get; init; }

    public string? EncryptionCertificateName { get; init; }

    public OidcApplicationConfiguration[] Applications { get; init; } = [];
}
