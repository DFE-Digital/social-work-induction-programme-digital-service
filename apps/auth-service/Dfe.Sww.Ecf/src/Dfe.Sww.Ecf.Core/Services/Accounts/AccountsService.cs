using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public class AccountsService(EcfDbContext dbContext) : IAccountsService
{
    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        var accounts = await dbContext.Persons.ToListAsync();
        return accounts;
    }
    public async Task<Person?> GetByIdAsync(Guid id)
    {
        var account = await dbContext.Persons.FirstOrDefaultAsync(x => x.PersonId == id);
        return account;
    }
}
