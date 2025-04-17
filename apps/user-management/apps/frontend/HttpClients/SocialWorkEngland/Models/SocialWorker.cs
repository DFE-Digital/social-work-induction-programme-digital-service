using System.Text.Json.Serialization;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

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

    [JsonPropertyName("Registration Number")]
    public string? RegistrationNumber { get; set; }

    [JsonPropertyName("Registered Name")]
    public string? RegisteredName { get; set; }

    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    [JsonPropertyName("Town of employment")]
    public string? TownOfEmployment { get; set; }

    [JsonPropertyName("Registered from")]
    public DateTime? RegisteredFrom { get; set; }

    [JsonPropertyName("Registered until")]
    public DateTime? RegisteredUntil { get; set; }

    [JsonPropertyName("Registered")]
    public bool? Registered { get; set; }
}
