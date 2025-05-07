using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public interface IAccountsService
{
    Task<PaginationResult<PersonDto>> GetAllAsync(PaginationRequest request, Guid organisationId);

    Task<PersonDto?> GetByIdAsync(Guid id);

    Task<PersonDto> CreateAsync(Person person);
    Task<PersonDto?> UpdateAsync(Person person);
    Task<PersonDto?> DeleteAsync(Guid id);
}
