using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;

/// <inheritdoc />
public class InvalidErrorResponse : ISocialWorkerResponse
{
    /// <inheritdoc />
    public SocialWorkerResponse MapResponse(SocialWorker? socialWorker)
    {
        return new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.OK,
                ErrorMessage = "Invalid Request"
            }
        };
    }
}
