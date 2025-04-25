using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Options;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService;

public class MoodleServiceClient : IMoodleServiceClient
{
    public MoodleServiceClient(HttpClient httpClient, IOptions<MoodleClientOptions> clientOptions)
    {
        HttpClient = httpClient;
        Options = clientOptions.Value;
        User = new UserOperations(this);
    }

    internal HttpClient HttpClient { get; init; }

    internal MoodleClientOptions Options { get; init; }

    public IUserOperations User { get; init; }
}
