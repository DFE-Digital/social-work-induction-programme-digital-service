using DfeSwwEcf.SweApiSimulator.Models;

namespace DfeSwwEcf.SweApiSimulator.Services.Interfaces;

/// <summary>
/// Social Worker Data Service
/// </summary>
public interface ISocialWorkerDataService
{
    /// <summary>
    /// Get a social worker by their ID
    /// </summary>
    /// <param name="swId"></param>
    /// <returns>A single social worker record</returns>
    public SocialWorker? GetById(int swId);
}
