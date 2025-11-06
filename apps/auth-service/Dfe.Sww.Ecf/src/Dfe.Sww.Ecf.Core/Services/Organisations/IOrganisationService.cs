using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Organisations;

public interface IOrganisationService
{
    Task<PaginationResult<OrganisationDto>> GetAllAsync(PaginationRequest request);
    Task<OrganisationDto?> GetByIdAsync(Guid id);
    Task<OrganisationDto> CreateAsync(Organisation organisation);
    Task<OrganisationDto?> GetByLocalAuthorityCodeAsync(int localAuthorityCode);
}
