using DfeSwwEcf.SweApiSimulator.Models;

namespace DfeSwwEcf.SweApiSimulator.Services.Interfaces;

/// <summary>
/// Social Worker Service
/// </summary>
public interface ISocialWorkerService
{
    /// <summary>
    /// Get a social worker by their ID
    /// </summary>
    /// <param name="swId"></param>
    /// <returns>A single social worker record</returns>
    public SocialWorkerResponse? GetById(string? swId);
}
