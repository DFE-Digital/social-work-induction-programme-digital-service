using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Models;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Interfaces;

public interface ISocialWorkersOperations
{
    /// <summary>
    /// Get a social worker by their SWE ID
    /// </summary>
    /// <param name="id"></param>
    Task<SocialWorker?> GetByIdAsync(int id);
}
