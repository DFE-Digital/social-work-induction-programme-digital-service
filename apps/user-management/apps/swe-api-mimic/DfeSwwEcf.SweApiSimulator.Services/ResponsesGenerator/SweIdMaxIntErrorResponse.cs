using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;

/// <inheritdoc />
public class SweIdMaxIntErrorResponse : ISocialWorkerResponse
{
    /// <inheritdoc />
    public SocialWorkerResponse MapResponse(SocialWorker? socialWorker)
    {
        return new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorMessage =
                    "Internal server error: One or more errors occurred. (Value was either too large or too small for an Int32.)"
            }
        };
    }
}
