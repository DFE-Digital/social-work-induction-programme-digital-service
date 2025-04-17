using System.Text.Json.Serialization;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

public class NonIntSweIdResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
