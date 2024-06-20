using DfeSwwEcf.NotificationService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Notify.Interfaces;

namespace DfeSwwEcf.NotificationService.UnitTests.Services.EmailSenderTests;

public abstract class EmailSenderTestsTestBase
{
    private protected Mock<IAsyncNotificationClient> MockGovNotifyClient { get; }
    private protected Mock<ILogger<EmailNotificationCommand>> MockLogger { get; }
    private protected EmailNotificationCommand Sut { get; }

    protected EmailSenderTestsTestBase()
    {
        MockGovNotifyClient = new();
        MockLogger = new();
        Sut = new(MockGovNotifyClient.Object, MockLogger.Object);
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockGovNotifyClient.VerifyNoOtherCalls();
        MockLogger.VerifyNoOtherCalls();
    }
}
