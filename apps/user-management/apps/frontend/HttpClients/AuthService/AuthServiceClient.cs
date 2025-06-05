using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Services;
using Dfe.Sww.Ecf.Frontend.Services;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;

public class AuthServiceClient : IAuthServiceClient
{
    public AuthServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        HttpClient = httpClient;
        HttpContextService = new HttpContextService(httpContextAccessor);
        Users = new UsersOperations(this);
    }
    internal HttpClient HttpClient { get; init; }
    public IHttpContextService HttpContextService { get; init; }
    public IUsersOperations Users { get; init; }
}
