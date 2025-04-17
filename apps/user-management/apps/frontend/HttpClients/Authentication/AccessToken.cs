using System.Text.Json.Serialization;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;

public class AccessToken
{
    private static readonly TimeSpan Threshold = new(0, 5, 0);

    public AccessToken(string token, string scheme, int expiresInSeconds)
    {
        Token = token;
        ExpiresInSeconds = expiresInSeconds;
        Scheme = scheme;
    }

    [JsonPropertyName("access_token")]
    public string Token { get; }

    [JsonPropertyName("token_type")]
    public required string Scheme { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresInSeconds { get; }

    public DateTime Expires => DateTime.UtcNow.AddSeconds(ExpiresInSeconds);

    public bool Expired => (Expires - DateTime.UtcNow).TotalSeconds <= Threshold.TotalSeconds;
}
