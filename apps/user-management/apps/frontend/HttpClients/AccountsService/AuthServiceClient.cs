using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Operations;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService;

public class AuthServiceClient : IAuthServiceClient
{
    public AuthServiceClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
        Accounts = new AccountsOperations(this);
    }

    internal HttpClient HttpClient { get; init; }

    public IAccountsOperations Accounts { get; init; }
}
