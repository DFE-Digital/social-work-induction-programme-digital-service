using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

public interface ILocalAuthorityOperations
{
    Task<Organisation?> GetByLocalAuthorityCodeAsync(int localAuthorityCode);
}
