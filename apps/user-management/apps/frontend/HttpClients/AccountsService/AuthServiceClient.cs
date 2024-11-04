using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Options;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService;

public class AuthServiceClient : IAuthServiceClient
{
    public AuthServiceClient(HttpClient httpClient, IOptions<AuthClientOptions> clientOptions)
    {
        HttpClient = httpClient;
        Options = clientOptions.Value;
        Accounts = new AccountsOperations(this);
    }

    internal HttpClient HttpClient { get; init; }

    internal AuthClientOptions Options { get; init; }

    public IAccountsOperations Accounts { get; init; }
}
