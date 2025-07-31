using System.Text;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Operations;

public class NotificationOperations(NotificationServiceClient notificationServiceClient)
    : INotificationOperations
{
    private readonly NotificationServiceClient _notificationServiceClient =
        notificationServiceClient;

    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };

    public async Task<NotificationResponse> SendEmailAsync(NotificationRequest request)
    {
        var route = _notificationServiceClient.Options.Routes.Notification.SendEmail;

        using var content = new StringContent(
            JsonSerializer.Serialize(request, SerializerOptions),
            Encoding.UTF8,
            "application/json"
        );

        var httpResponse = await _notificationServiceClient.HttpClient.PostAsync(route, content);

        return new NotificationResponse { StatusCode = httpResponse.StatusCode };
    }
}
