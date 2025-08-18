using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public abstract class CreateOrganisationJourneyServiceTestBase
{
    private protected const string CreateOrganisationSessionKey = "_createOrganisation";

    private protected readonly CreateOrganisationJourneyService Sut;

    protected CreateOrganisationJourneyServiceTestBase()
    {
        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        Sut = new CreateOrganisationJourneyService(
            httpContextAccessor,
            MockOrganisationService.Object,
            MockEmailService.Object
        );
    }

    private protected HttpContext HttpContext { get; } = new DefaultHttpContext
    {
        Request = { Headers = { Referer = "test-referer" } },
        Session = new MockHttpSession()
    };

    private protected Mock<IOrganisationService> MockOrganisationService { get; } = new();
    private protected Mock<IEmailService> MockEmailService { get; } = new();

    private protected OrganisationBuilder OrganisationBuilder { get; } = new();

    private protected AccountBuilder AccountBuilder { get; } = new();

    private protected void VerifyAllNoOtherCall()
    {
        MockOrganisationService.VerifyNoOtherCalls();
        MockEmailService.VerifyNoOtherCalls();
    }
}
