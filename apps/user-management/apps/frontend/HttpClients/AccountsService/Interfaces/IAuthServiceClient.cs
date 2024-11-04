namespace Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;

public interface IAuthServiceClient
{
    public IAccountsOperations Accounts { get; init; }
}
