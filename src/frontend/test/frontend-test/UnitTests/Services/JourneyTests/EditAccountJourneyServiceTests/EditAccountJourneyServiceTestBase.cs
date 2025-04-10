using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.Journeys;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Builders;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public abstract class EditAccountJourneyServiceTestBase
{
    private protected static string EditAccountSessionKey(Guid id) => "_editAccount-" + id;

    private protected AccountBuilder AccountBuilder { get; }
    private protected AccountDetailsFaker AccountDetailsFaker { get; }
    private protected HttpContext HttpContext { get; }
    private protected ITempDataDictionary TempData { get; }
    private protected Mock<IAccountService> MockAccountService { get; }

    private protected EditAccountJourneyService Sut;

    protected EditAccountJourneyServiceTestBase()
    {
        AccountBuilder = new();
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
