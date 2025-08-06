using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface IAccountsOperations
{
    Task<PaginationResult<Person>> GetAllAsync(PaginationRequest request, Guid? organisationId = null);

    Task<Person?> GetByIdAsync(Guid guid);

    Task<Person> CreateAsync(CreatePersonRequest createPersonRequest);

    Task<string> GetLinkingTokenByAccountIdAsync(Guid accountId);

    Task<Person> UpdateAsync(UpdatePersonRequest updatePersonRequest);
}
