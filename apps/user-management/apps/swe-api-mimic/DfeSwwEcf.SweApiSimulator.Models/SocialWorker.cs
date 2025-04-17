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
    public string? Id =>
        RegistrationNumber is not null && RegistrationNumber.StartsWith("SW")
            ? RegistrationNumber.Remove(0, 2)
            : RegistrationNumber;

    /// <summary>
    /// Registration Number
    /// </summary>
    [Name("Registration Number")]
    [JsonPropertyName("Registration Number")]
    public string? RegistrationNumber { get; set; }

    /// <summary>
    /// Registered Name
    /// </summary>
    [Name("Registered Name")]
    [JsonPropertyName("Registered Name")]
    public string? RegisteredName { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [Name("Status")]
    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    /// <summary>
    /// Town of employment
    /// </summary>
    [Name("Town of employment")]
    [JsonPropertyName("Town of employment")]
    public string? TownOfEmployment { get; set; }

    /// <summary>
    /// Registered from
    /// </summary>
    [Name("Registered from")]
    [JsonPropertyName("Registered from")]
    public DateTime? RegisteredFrom { get; set; }

    /// <summary>
    /// Registered until
    /// </summary>
    [Name("Registered until")]
    [JsonPropertyName("Registered until")]
    public DateTime? RegisteredUntil { get; set; }

    /// <summary>
    /// Annotations
    /// </summary>
    [Name("Annotations")]
    [JsonPropertyName("Annotations")]
    public List<string>? Annotations { get; set; } = [];

    /// <summary>
    /// Registered
    /// </summary>
    [Name("Registered")]
    [JsonPropertyName("Registered")]
    public string? Registered { get; set; }
}
