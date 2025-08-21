using System.Text;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Operations;

public class NotificationOperations(HttpClient httpClient, NotificationRoutes routes, ILogger logger, FeatureFlags featureFlags)
    : INotificationOperations
{
    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };

    public async Task<NotificationResponse> SendEmailAsync(NotificationRequest request)
    {
        Log("Sending email...");
        if (featureFlags.EnablePlusEmailStripping)
        {
            Log("Stripping 'plus' tag from email address");
            var email = request.EmailAddress;
            var atIndex = email.IndexOf('@');
            var plusIndex = email.IndexOf('+', 0, atIndex);
            request.EmailAddress = email[..plusIndex] + email[atIndex..];
        }

        using var content = new StringContent(
            JsonSerializer.Serialize(request, SerializerOptions),
            Encoding.UTF8,
            "application/json"
        );

        var route = routes.SendEmail;
        var httpResponse = await httpClient.PostAsync(route, content);

        if (!httpResponse.IsSuccessStatusCode)
        {
            Log($"Failed to send email with template ID {request.TemplateId}", LogLevel.Error);
            return new NotificationResponse { StatusCode = httpResponse.StatusCode };
        }

        Log("Email sent successfully");
        return new NotificationResponse { StatusCode = httpResponse.StatusCode };
    }

    private void Log(string message, LogLevel logLevel = LogLevel.Information)
    {
        logger.Log(logLevel, "[NotificationOperations] - {message}", message);
    }
}
