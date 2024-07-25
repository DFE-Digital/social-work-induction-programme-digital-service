using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace DfeSwwEcf.SweApiSimulator.Models;

/// <summary>
/// Social Worker
/// </summary>
public class SocialWorker
{
    /// <summary>
    /// The SWE ID
    /// </summary>
    [JsonIgnore]
    public int Id => RegistrationNumber.StartsWith("SW")
        ? Convert.ToInt32(RegistrationNumber?.Remove(0, 2))
        : Convert.ToInt32(RegistrationNumber);

    /// <summary>
    /// Registration Number
    /// </summary>
    [Name("Registration Number")]
    public string? RegistrationNumber { get; set; }

    /// <summary>
    /// Registered Name
    /// </summary>
    [Name("Registered Name")]
    public string? RegisteredName { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [Name("Status")]
    public string? Status { get; set; }

    /// <summary>
    /// Town of employment
    /// </summary>
    [Name("Town of employment")]
    public string? TownOfEmployment { get; set; }

    /// <summary>
    /// Registered from
    /// </summary>
    [Name("Registered from")]
    public DateTime? RegisteredFrom { get; set; }

    /// <summary>
    /// Registered until
    /// </summary>
    [Name("Registered until")]
    public DateTime? RegisteredUntil { get; set; }

    /// <summary>
    /// Registered
    /// </summary>
    [Name("Registered")]
    public bool? Registered { get; set; }
}
