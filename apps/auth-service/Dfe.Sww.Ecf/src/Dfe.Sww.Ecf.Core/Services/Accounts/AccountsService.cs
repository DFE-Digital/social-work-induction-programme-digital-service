using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public class AccountsService(EcfDbContext dbContext, IClock clock) : IAccountsService
{
    public async Task<PaginationResult<PersonDto>> GetAllAsync(PaginationRequest request)
    {
        var accounts = dbContext.Persons.AsQueryable();
        var totalItems = await accounts.CountAsync();

        var paginatedResults = await accounts
            .Include(p => p.PersonRoles)
            .ThenInclude(pr => pr.Role)
            .Skip(request.Offset)
            .Take(request.PageSize)
            .Select(x => x.ToDto())
            .ToListAsync();

        var response = new PaginationResult<PersonDto>
        {
            Records = paginatedResults,
            MetaData = new PaginationMetaData(request.Offset, request.PageSize, totalItems),
        };

        return response;
    }

    public async Task<PersonDto?> GetByIdAsync(Guid id)
    {
        var account = await dbContext
            .Persons.Include(p => p.PersonRoles)
            .ThenInclude(pr => pr.Role)
            .FirstOrDefaultAsync(x => x.PersonId == id);
        return account?.ToDto();
    }

    public async Task<PersonDto> CreateAsync(Person person)
    {
        person.CreatedOn = clock.UtcNow;
        await dbContext.Persons.AddAsync(person);
        await dbContext.SaveChangesAsync();

        // Ensure roles are loaded
        await dbContext
            .Entry(person)
            .Collection(x => x.PersonRoles)
            .Query()
            .Include(pr => pr.Role)
            .LoadAsync();

        return person.ToDto();
    }
}
