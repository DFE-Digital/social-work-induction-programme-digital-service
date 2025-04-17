using Dfe.Sww.Ecf.Frontend.HttpClients.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;

public class SocialWorkEnglandClientOptions : HttpClientOptions
{
    public required SweApiRoutes Routes { get; init; }

    public bool BypassSweChecks { get; init; } = false;
}
