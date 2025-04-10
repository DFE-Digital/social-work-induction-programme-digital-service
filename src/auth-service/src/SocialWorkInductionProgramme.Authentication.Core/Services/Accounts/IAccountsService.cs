using SocialWorkInductionProgramme.Authentication.Core.DataStore.Postgres.Models;
using SocialWorkInductionProgramme.Authentication.Core.Models.Pagination;

namespace SocialWorkInductionProgramme.Authentication.Core.Services.Accounts;

public interface IAccountsService
{
    Task<PaginationResult<PersonDto>> GetAllAsync(PaginationRequest request, Guid organisationId);

    Task<PersonDto?> GetByIdAsync(Guid id);

    Task<PersonDto> CreateAsync(Person person);
    Task<PersonDto?> UpdateAsync(Person person);
}
