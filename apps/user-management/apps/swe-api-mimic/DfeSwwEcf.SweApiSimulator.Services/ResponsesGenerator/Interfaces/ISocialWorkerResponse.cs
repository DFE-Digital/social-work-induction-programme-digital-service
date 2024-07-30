using DfeSwwEcf.SweApiSimulator.Models;

namespace DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;

/// <summary>
/// Social Worker Response
/// </summary>
public interface ISocialWorkerResponse
{
    /// <summary>
    /// Maps a SWE ID to a certain response
    /// </summary>
    /// <param name="socialWorker"></param>
    /// <returns>A response containing a successful or error response details</returns>
    SocialWorkerResponse MapResponse(SocialWorker? socialWorker = null);
}
