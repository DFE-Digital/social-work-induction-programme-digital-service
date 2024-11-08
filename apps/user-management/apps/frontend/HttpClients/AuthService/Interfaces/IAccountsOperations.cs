using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface IAccountsOperations
{
    Task<IList<Person>> GetAllAsync();

    Task<Person> GetByIdAsync(Guid guid);

    Task<string> GetLinkingTokenByAccountIdAsync(Guid accountId);
}
