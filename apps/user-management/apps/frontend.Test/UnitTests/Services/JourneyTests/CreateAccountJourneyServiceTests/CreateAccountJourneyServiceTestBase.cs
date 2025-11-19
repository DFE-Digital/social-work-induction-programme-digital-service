using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public abstract class CreateAccountJourneyServiceTestBase
{
    private protected const string CreateAccountSessionKey = "_createAccount";

    private protected readonly CreateAccountJourneyService Sut;

    protected CreateAccountJourneyServiceTestBase()
    {
        Sut = new CreateAccountJourneyService(
            new HttpContextAccessor { HttpContext = HttpContext },
            MockAccountService.Object,
            MockOrganisationService.Object,
            new FakeLinkGenerator(),
            MockEmailService.Object
        );
    }

    private protected AccountBuilder AccountBuilder { get; } = new();
    private protected OrganisationBuilder OrganisationBuilder { get; } = new();

    private protected HttpContext HttpContext { get; } = new DefaultHttpContext
    {
        Request = { Headers = { Referer = "test-referer" } },
        Session = new MockHttpSession()
    };

    private protected Mock<IAccountService> MockAccountService { get; } = new();
    private protected Mock<IOrganisationService> MockOrganisationService { get; } = new();
    private protected Mock<IEmailService> MockEmailService { get; } = new();

    private protected Mock<IOptions<FeatureFlags>> MockFeatureFlags { get; } = new();

    private protected void VerifyAllNoOtherCall()
    {
        MockAccountService.VerifyNoOtherCalls();
        MockEmailService.VerifyNoOtherCalls();
    }
}
