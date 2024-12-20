using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.EmailServices;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.EmailServiceTests.LinkingEmailServiceTests;

public abstract class LinkingEmailServiceTestBase
{
    private protected AccountBuilder AccountBuilder { get; }

    private protected Mock<INotificationServiceClient> MockNotificationClient { get; }

    private protected Mock<IOptions<EmailTemplateOptions>> MockEmailTemplateOptions { get; }

    private protected LinkingEmailService Sut;

    protected LinkingEmailServiceTestBase()
    {
        AccountBuilder = new();
        MockNotificationClient = new();
        MockEmailTemplateOptions = new();

        Sut = new(MockNotificationClient.Object, MockEmailTemplateOptions.Object);
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockNotificationClient.VerifyNoOtherCalls();
    }
}
