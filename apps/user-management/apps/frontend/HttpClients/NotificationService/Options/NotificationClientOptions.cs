using Dfe.Sww.Ecf.Frontend.HttpClients.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;

public class NotificationClientOptions : HttpClientOptions
{
    public required NotificationServiceRoutes Routes { get; init; }
}
