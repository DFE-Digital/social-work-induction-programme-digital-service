using SocialWorkInductionProgramme.Frontend.HttpClients.Authentication;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.Models;

public abstract class HttpClientOptions
{
    public required string BaseUrl { get; set; }

    public required ClientCredentials ClientCredentials { get; set; }
}
