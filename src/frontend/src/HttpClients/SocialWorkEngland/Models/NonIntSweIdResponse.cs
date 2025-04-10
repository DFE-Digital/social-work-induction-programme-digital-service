using System.Text.Json.Serialization;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Models;

public class NonIntSweIdResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
