using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;

/// <inheritdoc />
public class SocialWorkerResponseFactory : ISocialWorkerResponseFactory
{
    /// <inheritdoc />
    public ISocialWorkerResponse Create(string? sweId, SocialWorker? socialWorker)
    {
        // Check if the SWE ID provided in the request is null
        if (sweId is null)
        {
            return new SweIdNullErrorResponse();
        }

        // Check if the SWE ID provided in the request is a valid Int32
        var isLong = long.TryParse(sweId, out var longSweNumber);
        if (isLong && longSweNumber > int.MaxValue)
        {
            return new SweIdMaxIntErrorResponse();
        }

        // Check if the SWE ID provided in the request is a positive int
        var isInt = int.TryParse(sweId, out var sweNumber);
        if (!isInt || sweNumber <= 0 || sweId.StartsWith('0'))
        {
            return new NonIntSweIdResponse();
        }

        // Check if the SWE ID provided in the request is found in the CSV file
        if (socialWorker is null)
        {
            return new NotFoundResponse();
        }

        // Check if the SWE ID provided in the request is mapped to be an invalid response
        if (SweIdResponseConstants.InvalidResponseIds.Contains(sweId))
        {
            return new InvalidErrorResponse();
        }

        // If none of the above applies, return the found object
        return new ValidResponse();
    }
}
