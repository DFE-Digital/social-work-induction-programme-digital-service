using System.Net;
using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Interfaces;
using SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Models;
using Moq;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Services;

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
