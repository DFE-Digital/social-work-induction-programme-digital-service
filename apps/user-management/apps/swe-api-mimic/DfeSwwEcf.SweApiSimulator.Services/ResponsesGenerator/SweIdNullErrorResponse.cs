using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;

/// <inheritdoc />
public class SweIdNullErrorResponse : ISocialWorkerResponse
{
    /// <inheritdoc />
    public SocialWorkerResponse MapResponse(SocialWorker? socialWorker)
    {
        return new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
                ErrorMessage = "Please provide a non-null value"
            }
        };
    }
}
