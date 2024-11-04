using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;

public interface IAccountsOperations
{
    Task<Person> GetByIdAsync(Guid guid);
}
