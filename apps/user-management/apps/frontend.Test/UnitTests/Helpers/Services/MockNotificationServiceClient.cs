using System.Net;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;

public class MockNotificationServiceClient : Mock<INotificationServiceClient>
{
    public Mock<INotificationOperations> MockNotificationsOperations { get; }

    public MockNotificationServiceClient()
    {
        MockNotificationsOperations = new Mock<INotificationOperations>();
        SetupMockNotificationsOperations();
    }

    private void SetupMockNotificationsOperations()
    {
        MockNotificationsOperations
            .Setup(x => x.SendEmailAsync(It.IsAny<NotificationRequest>()))
            .ReturnsAsync(new NotificationResponse { StatusCode = HttpStatusCode.OK });
        Setup(x => x.Notification).Returns(MockNotificationsOperations.Object);
    }
}
