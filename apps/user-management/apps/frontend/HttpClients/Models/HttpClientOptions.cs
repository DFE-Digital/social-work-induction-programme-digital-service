using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.Models;

public abstract class HttpClientOptions
{
    public required string BaseUrl { get; set; }

    public required ClientCredentials ClientCredentials { get; set; }
}
