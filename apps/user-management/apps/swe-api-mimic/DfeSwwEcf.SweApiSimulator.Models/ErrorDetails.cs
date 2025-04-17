using System.Net;

namespace DfeSwwEcf.SweApiSimulator.Models;

/// <summary>
/// Error Response
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// Error Message
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The HTTP Status Code
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; set; }
}
