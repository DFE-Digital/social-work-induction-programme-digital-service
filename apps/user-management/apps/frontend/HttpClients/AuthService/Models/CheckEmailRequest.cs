using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;

[PublicAPI]
public record CheckEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
}
