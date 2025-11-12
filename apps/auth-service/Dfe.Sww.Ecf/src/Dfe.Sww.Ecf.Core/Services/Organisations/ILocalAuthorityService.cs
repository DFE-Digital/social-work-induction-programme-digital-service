namespace Dfe.Sww.Ecf.Core.Services.Organisations;

public interface ILocalAuthorityService
{
    Task<LocalAuthorityDto?> GetByCodeAsync(int localAuthorityCode);
}
