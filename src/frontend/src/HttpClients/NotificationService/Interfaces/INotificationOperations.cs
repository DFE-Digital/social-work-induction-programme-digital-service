using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Models;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Interfaces;

public interface INotificationOperations
{
    Task<NotificationResponse> SendEmailAsync(NotificationRequest request);
}
