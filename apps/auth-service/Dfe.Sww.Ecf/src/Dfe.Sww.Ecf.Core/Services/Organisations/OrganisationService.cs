using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Organisations;

public class OrganisationService(EcfDbContext dbContext) : IOrganisationService
{
    public async Task<PaginationResult<OrganisationDto>> GetAllAsync(PaginationRequest request)
    {
        var organisations = dbContext.Organisations;

        var totalItems = await organisations.CountAsync();

        var paginatedResults = await organisations
            .Skip(request.Offset)
            .Take(request.PageSize)
            .Select(x => x.ToDto())
            .ToListAsync();

        var response = new PaginationResult<OrganisationDto>
        {
            Records = paginatedResults,
            MetaData = new PaginationMetaData(request.Offset, request.PageSize, totalItems),
        };

        return response;
    }
}
