using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public abstract class EditUserJourneyServiceTestBase
{
    private protected static string EditUserSessionKey(Guid id) => "_editUser-" + id;

    private protected UserBuilder UserBuilder { get; }
    private protected UserDetailsFaker UserDetailsFaker { get; }
    private protected HttpContext HttpContext { get; }
    private protected ITempDataDictionary TempData { get; }
    private protected Mock<IUserService> MockUserService { get; }

    private protected EditUserJourneyService Sut;

    protected EditUserJourneyServiceTestBase()
    {
        UserBuilder = new();
        UserDetailsFaker = new();

        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        TempData = new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>());
        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        MockUserService = new();

        Sut = new(httpContextAccessor, MockUserService.Object);
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockUserService.VerifyNoOtherCalls();
    }
}
