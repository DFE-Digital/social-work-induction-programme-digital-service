using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;

public class OidcApplicationConfiguration
{
    public string? ClientId { get; [UsedImplicitly] init; }

    public string? ClientSecret { get; [UsedImplicitly] init; }

    public string ClientType { get; [UsedImplicitly] init; } = "confidential";

    public string? DisplayName { get; [UsedImplicitly] init; }

    public Uri[] RedirectUris { get; [UsedImplicitly] init; } = [];

    public Uri[] PostLogoutRedirectUris { get; [UsedImplicitly] init; } = [];

    public string[] AllowedScopes { get; [UsedImplicitly] init; } = [];

    public string[] AllowedEndpoints { get; [UsedImplicitly] init; } = [];

    public string[] AllowedGrantTypes { get; [UsedImplicitly] init; } = [];

    public string[] AllowedResponseTypes { get; [UsedImplicitly] init; } = [];

    public bool? RequirePkce { get; [UsedImplicitly] init; } = false;
}
