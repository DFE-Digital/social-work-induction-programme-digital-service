using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.EmailServiceTests;

public abstract class EmailServiceTestBase
{
    private protected AccountFaker AccountFaker { get; }

    private protected Mock<INotificationServiceClient> MockNotificationClient { get; }

    private protected Mock<IOptions<EmailTemplateOptions>> MockEmailTemplateOptions { get; }

    private protected EmailService Sut;

    protected EmailServiceTestBase()
    {
        AccountFaker = new();
        MockNotificationClient = new();
        MockEmailTemplateOptions = new();

        Sut = new(MockNotificationClient.Object, MockEmailTemplateOptions.Object);
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockNotificationClient.VerifyNoOtherCalls();
    }
}
