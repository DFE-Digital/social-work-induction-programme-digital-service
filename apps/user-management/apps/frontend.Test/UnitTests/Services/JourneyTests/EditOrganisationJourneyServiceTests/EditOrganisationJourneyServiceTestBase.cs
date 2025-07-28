using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public abstract class EditOrganisationJourneyServiceTestBase
{
    private protected string EditOrganisationSessionKey(Guid id) => "_editOrganisation-" + id;
    private protected HttpContext HttpContext { get; }
    private protected OrganisationBuilder OrganisationBuilder { get; }
    private protected AccountBuilder AccountBuilder { get; }

    private protected Mock<IOrganisationService> MockOrganisationService { get; }
    private protected Mock<IAccountService> MockAccountService { get; }

    private protected readonly EditOrganisationJourneyService Sut;

    protected EditOrganisationJourneyServiceTestBase()
    {
        OrganisationBuilder = new OrganisationBuilder();
        AccountBuilder = new AccountBuilder();
        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        MockOrganisationService = new();
        MockAccountService = new();

        Sut = new EditOrganisationJourneyService(
            httpContextAccessor,
            MockOrganisationService.Object,
            MockAccountService.Object
        );
    }
}
