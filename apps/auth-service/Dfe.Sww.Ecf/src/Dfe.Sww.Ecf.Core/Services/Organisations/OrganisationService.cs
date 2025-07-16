using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;

namespace Dfe.Sww.Ecf.Core.Services.Organisations;

public class OrganisationService(EcfDbContext dbContext, IClock clock) : IOrganisationService
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

    public async Task<OrganisationDto?> GetByIdAsync(Guid id)
    {
        var organisation = await dbContext
            .Organisations
            .FirstOrDefaultAsync(p => p.OrganisationId == id);

        return organisation?.ToDto();
    }

    public async Task<OrganisationDto> CreateAsync(Organisation organisation)
    {
        organisation.CreatedOn = clock.UtcNow;
        await dbContext.Organisations.AddAsync(organisation);
        await dbContext.SaveChangesAsync();

        return organisation.ToDto();
    }
}
