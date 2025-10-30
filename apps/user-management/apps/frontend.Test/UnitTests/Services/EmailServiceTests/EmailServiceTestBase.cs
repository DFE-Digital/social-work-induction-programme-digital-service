using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Configuration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.EmailServiceTests;

public abstract class EmailServiceTestBase
{
    protected EmailServiceTestBase()
    {
        MockHttpContextAccessor.Setup(x => x.HttpContext).Returns(HttpContext);

        Sut = new EmailService(
            MockAuthServiceClient.Object,
            MockNotificationServiceClient.Object,
            LinkGenerator,
            MockAccountService.Object,
            MockEmailTemplateOptions.Object,
            MockHttpContextAccessor.Object,
            MockLogger.Object);
    }

    private protected AccountBuilder AccountBuilder { get; } = new();

    private protected HttpContext HttpContext { get; } = new DefaultHttpContext
    {
        Request = { Headers = { Referer = "test-referer" } },
        Session = new MockHttpSession()
    };

    private protected Mock<IHttpContextAccessor> MockHttpContextAccessor { get; } = new();

    private protected MockAuthServiceClient MockAuthServiceClient { get; } = new();
    private protected MockNotificationServiceClient MockNotificationServiceClient { get; } = new();
    private protected Mock<IAccountService> MockAccountService { get; } = new();
    private protected FakeLinkGenerator LinkGenerator { get; } = new();
    private protected MockEmailTemplateOptions MockEmailTemplateOptions { get; } = new();
    private Mock<ILogger<EmailService>> MockLogger { get; } = new();
    private protected EmailService Sut { get; }


    protected void VerifyNoOtherCalls()
    {
        MockAuthServiceClient.VerifyNoOtherCalls();
        MockNotificationServiceClient.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
    }
}
