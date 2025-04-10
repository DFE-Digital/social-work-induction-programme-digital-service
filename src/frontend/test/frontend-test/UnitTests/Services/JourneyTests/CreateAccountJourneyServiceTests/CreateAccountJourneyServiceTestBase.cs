using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.Journeys;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Builders;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Configuration;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Fakers;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public abstract class CreateAccountJourneyServiceTestBase
{
    private protected const string CreateAccountSessionKey = "_createAccount";
    private protected AccountBuilder AccountBuilder { get; }
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
        AccountBuilder = new();
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
