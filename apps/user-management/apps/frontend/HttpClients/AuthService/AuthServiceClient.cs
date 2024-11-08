using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;

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
