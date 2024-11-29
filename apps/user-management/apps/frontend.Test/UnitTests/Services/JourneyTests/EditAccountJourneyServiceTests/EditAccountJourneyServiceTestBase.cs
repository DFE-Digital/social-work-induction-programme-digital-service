using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public abstract class EditAccountJourneyServiceTestBase
{
    private protected static string EditAccountSessionKey(Guid id) => "_editAccount-" + id;

    private protected AccountFaker AccountFaker { get; }
    private protected AccountDetailsFaker AccountDetailsFaker { get; }
    private protected HttpContext HttpContext { get; }
    private protected ITempDataDictionary TempData { get; }
    private protected Mock<IAccountService> MockAccountService { get; }

    private protected EditAccountJourneyService Sut;

    protected EditAccountJourneyServiceTestBase()
    {
        AccountFaker = new();
        AccountDetailsFaker = new();

        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        TempData = new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>());
        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        MockAccountService = new();

        Sut = new(httpContextAccessor, MockAccountService.Object);
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockAccountService.VerifyNoOtherCalls();
    }
}
