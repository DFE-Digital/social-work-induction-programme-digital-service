using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public interface IAccountsService
{
    Task<PaginationResult<Person>> GetAllAsync(PaginationRequest request);

    Task<Person?> GetByIdAsync(Guid id);

    Task<Person> CreateAsync(Person person);
}
