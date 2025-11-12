using Dfe.Sww.Ecf.Core.DataStore.Postgres;

namespace Dfe.Sww.Ecf.Core.Services.Organisations;

public class LocalAuthorityService(EcfDbContext dbContext) : ILocalAuthorityService
{
    public async Task<LocalAuthorityDto?> GetByCodeAsync(int localAuthorityCode)
    {
        var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync(p =>
            p.OldLaCode == localAuthorityCode
        );

        return localAuthority?.ToDto();
    }
}
