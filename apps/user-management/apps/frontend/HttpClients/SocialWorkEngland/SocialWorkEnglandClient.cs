using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;

public class SocialWorkEnglandClient : ISocialWorkEnglandClient
{
    public SocialWorkEnglandClient(
        HttpClient httpClient,
        IOptions<SocialWorkEnglandClientOptions> clientOptions
    )
    {
        HttpClient = httpClient;
        Options = clientOptions.Value;
        SocialWorkers = new SocialWorkersOperations(this);
    }

    internal HttpClient HttpClient { get; init; }

    internal SocialWorkEnglandClientOptions Options { get; init; }

    public ISocialWorkersOperations SocialWorkers { get; init; }
}
