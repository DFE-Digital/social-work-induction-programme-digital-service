using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;
using Microsoft.Extensions.Options;
using Polly;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;

public class SocialWorkEnglandClient : ISocialWorkEnglandClient
{
    public SocialWorkEnglandClient(
        HttpClient httpClient,
        IOptions<SocialWorkEnglandClientOptions> clientOptions,
        [FromKeyedServices(nameof(SocialWorkEnglandClient))]
            ResiliencePipeline<HttpResponseMessage> pipeline
    )
    {
        HttpClient = httpClient;
        Options = clientOptions.Value;
        SocialWorkers = new SocialWorkersOperations(this, pipeline);
    }

    internal HttpClient HttpClient { get; init; }

    internal SocialWorkEnglandClientOptions Options { get; init; }

    public ISocialWorkersOperations SocialWorkers { get; init; }
}
