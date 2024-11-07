using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;

public interface IAccountsOperations
{
    Task<IList<Person>> GetAllAsync();

    Task<Person> GetByIdAsync(Guid guid);
}
