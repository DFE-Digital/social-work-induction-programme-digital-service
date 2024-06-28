using DfeSwwEcf.NotificationService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Notify.Interfaces;

namespace DfeSwwEcf.NotificationService.Tests.UnitTests.Services.EmailSenderTests;

public abstract class EmailSenderTestsTestBase
{
    private protected Mock<IAsyncNotificationClient> MockGovNotifyClient { get; }

    private protected Mock<ILogger<EmailNotificationCommand>> MockLogger { get; }

    private protected EmailNotificationCommand Sut { get; }

    protected EmailSenderTestsTestBase()
    {
        MockGovNotifyClient = new Mock<IAsyncNotificationClient>();
        MockLogger = new Mock<ILogger<EmailNotificationCommand>>();
        Sut = new EmailNotificationCommand(MockGovNotifyClient.Object, MockLogger.Object);
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockGovNotifyClient.VerifyNoOtherCalls();
        MockLogger.VerifyNoOtherCalls();
    }
}
