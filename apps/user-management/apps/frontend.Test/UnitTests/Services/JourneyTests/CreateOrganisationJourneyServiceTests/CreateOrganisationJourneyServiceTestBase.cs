using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Configuration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public abstract class CreateOrganisationJourneyServiceTestBase
{
    private protected const string CreateOrganisationSessionKey = "_createOrganisation";

    private protected readonly CreateOrganisationJourneyService Sut;

    protected CreateOrganisationJourneyServiceTestBase()
    {
        OrganisationBuilder = new OrganisationBuilder();
        AccountBuilder = new AccountBuilder();
        MockOrganisationService = new Mock<IOrganisationService>();
        MockAccountService = new Mock<IAccountService>();
        MockAuthServiceClient = new MockAuthServiceClient();
        MockNotificationServiceClient = new MockNotificationServiceClient();
        MockEmailTemplateOptions = new MockEmailTemplateOptions();
        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };

        Sut = new CreateOrganisationJourneyService(
            httpContextAccessor,
            MockOrganisationService.Object,
            MockAuthServiceClient.Object,
            MockAccountService.Object,
            MockNotificationServiceClient.Object,
            MockEmailTemplateOptions.Object,
            new FakeLinkGenerator()
        );
    }

    private protected HttpContext HttpContext { get; }

    private protected Mock<IOrganisationService> MockOrganisationService { get; }
    private protected Mock<IAccountService> MockAccountService { get; }
    private protected MockAuthServiceClient MockAuthServiceClient { get; }
    private protected MockNotificationServiceClient MockNotificationServiceClient { get; }
    private protected MockEmailTemplateOptions MockEmailTemplateOptions { get; }

    private protected OrganisationBuilder OrganisationBuilder { get; }

    private protected AccountBuilder AccountBuilder { get; }
}
