using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Options;
using SocialWorkInductionProgramme.Frontend.HttpClients.Models;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Options;

public class NotificationClientOptions : HttpClientOptions
{
    public required NotificationServiceRoutes Routes { get; init; }
}
