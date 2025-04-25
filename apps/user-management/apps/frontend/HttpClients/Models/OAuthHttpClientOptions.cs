using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.Models;

public abstract class OAuthHttpClientOptions : HttpClientOptions
{
    public required ClientCredentials ClientCredentials { get; init; }
}
