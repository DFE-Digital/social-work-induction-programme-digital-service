namespace DfeSwwEcf.SweApiSimulator.Models;

/// <summary>
/// Error Response
/// </summary>
public class SocialWorkerResponse
{
    /// <summary>
    /// The Social Worker Details
    /// </summary>
    public SocialWorker? SocialWorker { get; set; }

    /// <summary>
    /// The ErrorDetails
    /// </summary>
    public ErrorDetails? ErrorDetails { get; set; }
}
