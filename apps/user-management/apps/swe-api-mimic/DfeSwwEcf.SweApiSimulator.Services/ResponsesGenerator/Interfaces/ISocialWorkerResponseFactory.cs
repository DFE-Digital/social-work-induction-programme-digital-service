using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator.Interfaces;

/// <summary>
/// Social Worker Response Factory
/// </summary>
public interface ISocialWorkerResponseFactory
{
    /// <summary>
    /// Creates the relevant concrete class for the given SWE ID
    /// </summary>
    /// <param name="sweId">The Social Worker ID passed in the request</param>
    /// <param name="socialWorker">The matching social worker record found</param>
    /// <returns></returns>
    public ISocialWorkerResponse Create(string? sweId, SocialWorker? socialWorker);
}
