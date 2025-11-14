using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface IOrganisationOperations
{
    Task<PaginationResult<OrganisationDto>> GetAllAsync(PaginationRequest request);
    Task<OrganisationDto> CreateAsync(CreateOrganisationRequest createOrganisationRequest);
    Task<OrganisationDto?> GetByIdAsync(Guid id);
    Task<bool> ExistsByLocalAuthorityCodeAsync(int localAuthorityCode);
}
