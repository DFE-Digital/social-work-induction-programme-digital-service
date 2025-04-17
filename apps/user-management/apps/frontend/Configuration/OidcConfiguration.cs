using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Configuration;

public sealed class OidcConfiguration
{
    public const string ConfigurationKey = "Oidc";

    [Required]
    public required string AuthorityUrl { get; init; }

    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }

    [Required]
    public required string[] Scopes { get; init; }

    [Required]
    public required string CallbackUrl { get; init; }

    [Required]
    public required string SignedOutCallbackUrl { get; init; }

    public string? CookieName { get; init; }

    public ushort SessionLifetimeMinutes { get; init; } = 60;

    public bool EnableDevelopmentBackdoor { get; init; }
}
