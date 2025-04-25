namespace Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;

public class ClientCredentials
{
    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required string AccessTokenUrl { get; init; }
}
