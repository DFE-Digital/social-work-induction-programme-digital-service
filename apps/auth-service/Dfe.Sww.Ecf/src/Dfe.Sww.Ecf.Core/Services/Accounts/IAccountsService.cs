using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public interface IAccountsService
{
    Task<PaginationResult<PersonDto>> GetAllAsync(PaginationRequest request);

    Task<PersonDto?> GetByIdAsync(Guid id);

    Task<PersonDto> CreateAsync(Person person);
}
