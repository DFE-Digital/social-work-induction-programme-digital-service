using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Configuration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public abstract class CreateAccountJourneyServiceTestBase
{
    private protected const string CreateAccountSessionKey = "_createAccount";
    private protected AccountBuilder AccountBuilder { get; }
    private protected HttpContext HttpContext { get; }
    private protected Mock<IAccountService> MockAccountService { get; }
    private protected MockNotificationServiceClient MockNotificationServiceClient { get; }
    private protected MockEmailTemplateOptions MockEmailTemplateOptions { get; }
    private protected MockAuthServiceClient MockAuthServiceClient { get; }
    private protected Mock<IMoodleServiceClient> MockMoodleServiceClient { get; }

    private protected CreateAccountJourneyService Sut;

    protected CreateAccountJourneyServiceTestBase()
    {
        AccountBuilder = new();

        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        MockAccountService = new Mock<IAccountService>();
        MockNotificationServiceClient = new MockNotificationServiceClient();
        MockEmailTemplateOptions = new MockEmailTemplateOptions();
        MockAuthServiceClient = new MockAuthServiceClient();
        MockMoodleServiceClient = new Mock<IMoodleServiceClient>();

        Sut = new CreateAccountJourneyService(
            httpContextAccessor,
            MockNotificationServiceClient.Object,
            MockEmailTemplateOptions.Object,
            MockAuthServiceClient.Object,
            MockAccountService.Object,
            MockMoodleServiceClient.Object,
            new FakeLinkGenerator()
        );
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockNotificationServiceClient.VerifyNoOtherCalls();
        MockEmailTemplateOptions.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
        MockMoodleServiceClient.VerifyNoOtherCalls();
    }
}
