using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
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
    private protected AccountFaker AccountFaker { get; }
    private protected AccountDetailsFaker AccountDetailsFaker { get; }
    private protected HttpContext HttpContext { get; }
    private protected ITempDataDictionary TempData { get; }
    private protected Mock<IAccountService> MockAccountService { get; }
    private protected MockNotificationServiceClient MockNotificationServiceClient { get; }
    private protected MockEmailTemplateOptions MockEmailTemplateOptions { get; }
    private protected MockAuthServiceClient MockAuthServiceClient { get; }

    private protected CreateAccountJourneyService Sut;

    protected CreateAccountJourneyServiceTestBase()
    {
        AccountFaker = new AccountFaker();
        AccountDetailsFaker = new AccountDetailsFaker();

        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        TempData = new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>());
        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        MockAccountService = new Mock<IAccountService>();
        MockNotificationServiceClient = new MockNotificationServiceClient();
        MockEmailTemplateOptions = new MockEmailTemplateOptions();
        MockAuthServiceClient = new MockAuthServiceClient();

        Sut = new CreateAccountJourneyService(
            httpContextAccessor,
            MockNotificationServiceClient.Object,
            MockEmailTemplateOptions.Object,
            MockAuthServiceClient.Object,
            MockAccountService.Object,
            new FakeLinkGenerator()
        );
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockNotificationServiceClient.VerifyNoOtherCalls();
        MockEmailTemplateOptions.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
    }
}
