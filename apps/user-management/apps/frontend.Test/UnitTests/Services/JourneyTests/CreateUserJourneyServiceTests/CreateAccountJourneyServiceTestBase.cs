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

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public abstract class CreateUserJourneyServiceTestBase
{
    private protected const string CreateUserSessionKey = "_createUser";
    private protected UserBuilder UserBuilder { get; }
    private protected UserDetailsFaker UserDetailsFaker { get; }
    private protected HttpContext HttpContext { get; }
    private protected ITempDataDictionary TempData { get; }
    private protected Mock<IUserService> MockUserService { get; }
    private protected MockNotificationServiceClient MockNotificationServiceClient { get; }
    private protected MockEmailTemplateOptions MockEmailTemplateOptions { get; }
    private protected MockAuthServiceClient MockAuthServiceClient { get; }

    private protected Mock<IMoodleServiceClient> MockMoodleServiceClient { get; }

    private protected CreateUserJourneyService Sut;

    protected CreateUserJourneyServiceTestBase()
    {
        UserBuilder = new();
        UserDetailsFaker = new UserDetailsFaker();

        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        TempData = new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>());
        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        MockUserService = new Mock<IUserService>();
        MockNotificationServiceClient = new MockNotificationServiceClient();
        MockEmailTemplateOptions = new MockEmailTemplateOptions();
        MockAuthServiceClient = new MockAuthServiceClient();
        MockMoodleServiceClient = new Mock<IMoodleServiceClient>();

        Sut = new CreateUserJourneyService(
            httpContextAccessor,
            MockNotificationServiceClient.Object,
            MockEmailTemplateOptions.Object,
            MockAuthServiceClient.Object,
            MockUserService.Object,
            MockMoodleServiceClient.Object,
            new FakeLinkGenerator()
        );
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockNotificationServiceClient.VerifyNoOtherCalls();
        MockEmailTemplateOptions.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
        MockUserService.VerifyNoOtherCalls();
        MockMoodleServiceClient.VerifyNoOtherCalls();
    }
}
