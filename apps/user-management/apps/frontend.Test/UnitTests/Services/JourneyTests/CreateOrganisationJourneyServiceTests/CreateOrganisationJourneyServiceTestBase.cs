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
    private protected HttpContext HttpContext { get; }

    private protected Mock<IOrganisationService> MockOrganisationService { get; }
    private protected Mock<IAccountService> MockAccountService { get; }

    private protected OrganisationBuilder OrganisationBuilder { get; }

    private protected AccountBuilder AccountBuilder { get; }

    private protected readonly CreateOrganisationJourneyService Sut;

    protected CreateOrganisationJourneyServiceTestBase()
    {
        OrganisationBuilder = new OrganisationBuilder();
        AccountBuilder = new AccountBuilder();
        MockOrganisationService = new();
        MockAccountService = new();
        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        Sut = new CreateOrganisationJourneyService(
            httpContextAccessor,
            MockOrganisationService.Object
        );
    }
}
