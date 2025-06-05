using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface IUserService
{
    public Task<PaginationResult<User>> GetAllAsync(PaginationRequest request);

    public Task<User?> GetByIdAsync(Guid id);

    public Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User updatedUser);
}
