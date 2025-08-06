namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;

public sealed class OneLoginOptions
{
    public const string ConfigurationKey = "OneLogin";

    public string Url { get; init; } = "";

    public string? ClientId { get; init; }

    public string? PrivateKeyPem { get; init; }
}
