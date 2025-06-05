namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface IAuthServiceClient
{
    public IUsersOperations Users { get; init; }

    public IHttpContextService HttpContextService { get; init; }
}
