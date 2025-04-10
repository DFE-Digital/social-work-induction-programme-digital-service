using SocialWorkInductionProgramme.Frontend.Services;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Interfaces;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Operations;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Services;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.AuthService;

public class AuthServiceClient : IAuthServiceClient
{
    public AuthServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        HttpClient = httpClient;
        HttpContextService = new HttpContextService(httpContextAccessor);
        Accounts = new AccountsOperations(this);
    }
    internal HttpClient HttpClient { get; init; }
    public IHttpContextService HttpContextService { get; init; }
    public IAccountsOperations Accounts { get; init; }
}
