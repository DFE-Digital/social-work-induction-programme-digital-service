using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;

/// <inheritdoc />
public class ValidResponse : ISocialWorkerResponse
{
    /// <inheritdoc />
    public SocialWorkerResponse MapResponse(SocialWorker? socialWorker)
    {
        return new SocialWorkerResponse { SocialWorker = socialWorker };
    }
}
