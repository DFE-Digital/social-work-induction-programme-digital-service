using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public abstract class EditAccountJourneyServiceTestBase
{
    private protected static string EditAccountSessionKey(Guid id)
    {
        return "_editAccount-" + id;
    }

    private protected AccountBuilder AccountBuilder { get; }
    private protected AccountDetailsFaker AccountDetailsFaker { get; }
    private protected HttpContext HttpContext { get; }
    private protected ITempDataDictionary TempData { get; }

    private protected Mock<IAccountService> MockAccountService { get; } = new();
    private protected Mock<IMoodleService> MockMoodleService { get; } = new();
    private protected Mock<IOptions<FeatureFlags>> MockFeatureFlags { get; } = new();


    private protected EditAccountJourneyService Sut;

    protected EditAccountJourneyServiceTestBase()
    {
        AccountBuilder = new AccountBuilder();
        AccountDetailsFaker = new AccountDetailsFaker();

        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        TempData = new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>());
        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        Sut = new EditAccountJourneyService(httpContextAccessor, MockAccountService.Object, new FakeLinkGenerator(), MockMoodleService.Object, MockFeatureFlags.Object);
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockAccountService.VerifyNoOtherCalls();
    }
}
