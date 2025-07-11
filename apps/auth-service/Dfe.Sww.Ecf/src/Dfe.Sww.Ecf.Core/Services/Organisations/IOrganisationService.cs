using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Organisations;

public interface IOrganisationService
{
    Task<PaginationResult<OrganisationDto>> GetAllAsync(PaginationRequest request);
}
