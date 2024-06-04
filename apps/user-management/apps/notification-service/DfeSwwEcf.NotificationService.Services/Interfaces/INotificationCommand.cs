using DfeSwwEcf.NotificationService.Models;

namespace DfeSwwEcf.NotificationService.Services.Interfaces;

/// <summary>
/// The Notification Command
/// </summary>
public interface INotificationCommand
{
    /// <summary>
    /// Sends a notification
    /// </summary>
    /// <param name="notificationRequest"></param>
    Task SendNotificationAsync(NotificationRequest notificationRequest);
}
