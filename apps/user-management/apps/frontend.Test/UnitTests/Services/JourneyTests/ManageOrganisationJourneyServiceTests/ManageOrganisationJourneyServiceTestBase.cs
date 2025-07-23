using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.ManageOrganisationJourneyServiceTests;

public abstract class ManageOrganisationJourneyServiceTestBase
{
    private protected const string ManageOrganisationSessionKey = "_manageOrganisation";
    private protected HttpContext HttpContext { get; }

    private protected Mock<IOrganisationService> MockOrganisationService { get; }
    private protected Mock<IAccountService> MockAccountService { get; }

    private protected OrganisationBuilder OrganisationBuilder { get; }

    private protected AccountBuilder AccountBuilder { get; }

    private protected readonly ManageOrganisationJourneyService Sut;

    protected ManageOrganisationJourneyServiceTestBase()
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

        Sut = new ManageOrganisationJourneyService(
            httpContextAccessor,
            MockOrganisationService.Object
        );
    }
}
