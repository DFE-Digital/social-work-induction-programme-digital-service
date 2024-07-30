using System.Diagnostics.CodeAnalysis;

namespace DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;

/// <summary>
/// Error Response Codes
/// </summary>
[ExcludeFromCodeCoverage]
public static class SweIdResponseConstants
{
    /// <summary>
    /// Invalid Ids - Error message returned from the API
    /// </summary>
    /// <returns>"Invalid Request"</returns>
    public static readonly IList<string> InvalidResponseIds = new List<string>
    {
        "4296",
        "95",
        "832"
    };

    /// <summary>
    /// Not Found IDs - Error message returned from the API - Corresponding objects not found
    /// </summary>
    /// <returns>"No SocialWorker found with this ID"</returns>
    public static readonly IList<string> NotFoundResponseIds = new List<string> { "99999999" };
}
