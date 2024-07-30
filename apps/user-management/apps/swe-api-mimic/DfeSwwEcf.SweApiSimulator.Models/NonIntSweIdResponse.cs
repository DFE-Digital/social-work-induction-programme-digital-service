using System.Text.Json.Serialization;

namespace DfeSwwEcf.SweApiSimulator.Models;

public class NonIntSweIdResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
