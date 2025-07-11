namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface IAuthServiceClient
{
    public IAccountsOperations Accounts { get; init; }
    public IOrganisationOperations Organisations { get; init; }

    public IHttpContextService HttpContextService { get; init; }
}
