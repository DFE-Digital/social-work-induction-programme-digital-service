using Dfe.Sww.Ecf.Frontend.HttpClients.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;

public class NotificationClientOptions : HttpClientOptions
{
    public required NotificationServiceRoutes Routes { get; init; }
    public string? FunctionKey { get; init; } = null;
}
