using Microsoft.Extensions.Options;
using Polly;
using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Operations;
using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Options;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland;

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
