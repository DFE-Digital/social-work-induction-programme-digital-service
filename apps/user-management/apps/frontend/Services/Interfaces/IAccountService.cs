using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface IAccountService
{
    public Task<PaginationResult<Account>> GetAllAsync(PaginationRequest request);

    public Task<Account> GetByIdAsync(Guid id);

    public Task<Account> CreateAsync(Account account);
}
