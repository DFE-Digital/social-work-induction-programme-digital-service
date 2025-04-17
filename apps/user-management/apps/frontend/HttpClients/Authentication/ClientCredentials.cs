namespace Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;

public class ClientCredentials
{
    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string AccessTokenUrl { get; set; }
}
