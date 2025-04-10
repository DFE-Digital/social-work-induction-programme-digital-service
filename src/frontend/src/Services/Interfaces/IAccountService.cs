using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Models.Pagination;
using SocialWorkInductionProgramme.Frontend.Models;

namespace SocialWorkInductionProgramme.Frontend.Services.Interfaces;

public interface IAccountService
{
    public Task<PaginationResult<Account>> GetAllAsync(PaginationRequest request);

    public Task<Account?> GetByIdAsync(Guid id);

    public Task<Account> CreateAsync(Account account);
    Task<Account> UpdateAsync(Account updatedAccount);
}
