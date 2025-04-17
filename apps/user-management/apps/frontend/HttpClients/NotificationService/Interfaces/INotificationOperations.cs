using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;

public interface INotificationOperations
{
    Task<NotificationResponse> SendEmailAsync(NotificationRequest request);
}
