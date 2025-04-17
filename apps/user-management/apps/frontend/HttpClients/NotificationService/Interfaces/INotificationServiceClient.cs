namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;

public interface INotificationServiceClient
{
    public INotificationOperations Notification { get; init; }
}
