using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public class AccountsService(EcfDbContext dbContext, IClock clock) : IAccountsService
{
    public async Task<PaginationResult<Person>> GetAllAsync(PaginationRequest request)
    {
        var accounts = dbContext.Persons.AsQueryable();
        var totalItems = await accounts.CountAsync();

        var paginatedResults = await accounts
            .Skip(request.Offset)
            .Take(request.PageSize)
            .ToListAsync();

        var response = new PaginationResult<Person>
        {
            Records = paginatedResults,
            MetaData = new PaginationMetaData(request.Offset, request.PageSize, totalItems),
        };

        return response;
    }

    public async Task<Person?> GetByIdAsync(Guid id)
    {
        var account = await dbContext.Persons.FirstOrDefaultAsync(x => x.PersonId == id);
        return account;
    }

    public async Task<Person> CreateAsync(Person person)
    {
        person.CreatedOn = clock.UtcNow;
        await dbContext.Persons.AddAsync(person);
        await dbContext.SaveChangesAsync();
        return person;
    }
}
