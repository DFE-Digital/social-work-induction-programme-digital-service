using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;

/// <inheritdoc />
public class NotFoundResponse : ISocialWorkerResponse
{
    /// <inheritdoc />
    public SocialWorkerResponse MapResponse(SocialWorker? socialWorker)
    {
        return new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                ErrorMessage = "No SocialWorker found with this ID"
            }
        };
    }
}
