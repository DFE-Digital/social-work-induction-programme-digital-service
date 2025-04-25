namespace Dfe.Sww.Ecf.Frontend.HttpClients.Models;

public abstract class ApiTokenHttpClientOptions : HttpClientOptions
{
    public required string ApiToken { get; init; }
}
