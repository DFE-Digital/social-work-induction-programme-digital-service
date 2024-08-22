using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;

public interface ISocialWorkersOperations
{
    /// <summary>
    /// Get a social worker by their SWE ID
    /// </summary>
    /// <param name="id"></param>
    Task<SocialWorker?> GetById(int id);
}
