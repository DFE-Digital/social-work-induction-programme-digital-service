using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public class AccountsService(EcfDbContext dbContext, IClock clock) : IAccountsService
{
    public async Task<PaginationResult<PersonDto>> GetAllAsync(PaginationRequest request, Guid organisationId)
    {
        var accounts = dbContext.Persons
            .OrderBy(x => x.FirstName)
            .Include(p => p.PersonOrganisations)
            .Where(p => p.PersonOrganisations.Any(o => o.OrganisationId == organisationId)
                        && p.DeletedOn.HasValue == false)
            .Select(p => p);

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
            .FirstOrDefaultAsync(p => p.PersonId == id
                                      && p.DeletedOn.HasValue == false);
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

    public async Task<PersonDto?> UpdateAsync(Person updatedAccount)
    {
        var account = await dbContext
            .Persons.Include(p => p.PersonRoles)
            .ThenInclude(pr => pr.Role)
            .FirstOrDefaultAsync(x => x.PersonId == updatedAccount.PersonId);

        if (account is null)
        {
            return null;
        }

        account.FirstName = updatedAccount.FirstName;
        account.LastName = updatedAccount.LastName;
        account.EmailAddress = updatedAccount.EmailAddress;
        account.Trn = updatedAccount.Trn;
        account.UpdatedOn = clock.UtcNow;
        account.Status = updatedAccount.Status;

        account.PersonRoles.Clear();
        foreach (var role in updatedAccount.PersonRoles)
        {
            account.PersonRoles.Add(role);
        }

        await dbContext.SaveChangesAsync();

        // Ensure roles are loaded
        await dbContext
            .Entry(account)
            .Collection(x => x.PersonRoles)
            .Query()
            .Include(pr => pr.Role)
            .LoadAsync();

        return account.ToDto();
    }

    public async Task<PersonDto?> DeleteAsync(Guid id)
    {
        var account = await dbContext
            .Persons.Include(p => p.PersonRoles)
            .ThenInclude(pr => pr.Role)
            .FirstOrDefaultAsync(x => x.PersonId == id);

        if (account is null)
        {
            return null;
        }

        account.DeletedOn = clock.UtcNow;
        await dbContext.SaveChangesAsync();

        // Ensure roles are loaded
        await dbContext
            .Entry(account)
            .Collection(x => x.PersonRoles)
            .Query()
            .Include(pr => pr.Role)
            .LoadAsync();

        return account.ToDto();
    }
}
