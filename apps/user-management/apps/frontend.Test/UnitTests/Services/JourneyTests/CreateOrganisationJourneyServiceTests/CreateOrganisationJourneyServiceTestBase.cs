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

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public abstract class CreateOrganisationJourneyServiceTestBase
{
    private protected const string CreateOrganisationSessionKey = "_createOrganisation";
    private protected HttpContext HttpContext { get; }

    private protected Mock<IOrganisationService> MockOrganisationService { get; }

    private protected readonly CreateOrganisationJourneyService Sut;

    private protected OrganisationBuilder OrganisationBuilder { get; }

    protected CreateOrganisationJourneyServiceTestBase()
    {
        OrganisationBuilder = new OrganisationBuilder();
        MockOrganisationService = new();
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
