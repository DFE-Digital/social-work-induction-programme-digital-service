using SocialWorkInductionProgramme.Frontend.HttpClients.Models;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Options;

public class SocialWorkEnglandClientOptions : HttpClientOptions
{
    public required SweApiRoutes Routes { get; init; }

    public bool BypassSweChecks { get; init; } = false;
}
