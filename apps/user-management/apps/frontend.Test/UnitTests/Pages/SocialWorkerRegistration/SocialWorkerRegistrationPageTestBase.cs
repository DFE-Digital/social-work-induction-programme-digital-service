using Bogus;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public abstract class SocialWorkerRegistrationPageTestBase
{
    private protected AccountBuilder AccountBuilder { get; }
    private protected GetEscwRegisterChangeLinksFaker GetEscwRegisterChangeLinksFaker { get; }
    private protected Mock<IEthnicGroupService> MockEthnicGroupService { get; }
    private protected Mock<IRegisterSocialWorkerJourneyService> MockRegisterSocialWorkerJourneyService { get; }
    private protected MockAuthServiceClient MockAuthServiceClient { get; }
    private protected Guid PersonId { get; }

    protected SocialWorkerRegistrationPageTestBase()
    {
        AccountBuilder = new();
        GetEscwRegisterChangeLinksFaker = new();
        PersonId = Guid.NewGuid();
        MockEthnicGroupService = new();
        MockRegisterSocialWorkerJourneyService = new();
        MockAuthServiceClient = new();

        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups).Returns(MockEthnicGroupService.Object);
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockRegisterSocialWorkerJourneyService.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
    }
}
