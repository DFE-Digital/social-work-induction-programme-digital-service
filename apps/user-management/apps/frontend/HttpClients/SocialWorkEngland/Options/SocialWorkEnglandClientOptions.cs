using Dfe.Sww.Ecf.Frontend.HttpClients.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;

public class SocialWorkEnglandClientOptions : HttpClientOptions
{
    public SweApiRoutes? Routes { get; init; }
}
