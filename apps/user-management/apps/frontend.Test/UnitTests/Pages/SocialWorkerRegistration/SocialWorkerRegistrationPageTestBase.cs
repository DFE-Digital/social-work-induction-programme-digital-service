using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public abstract class SocialWorkerRegistrationPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    private protected Mock<IRegisterSocialWorkerJourneyService> MockRegisterSocialWorkerJourneyService { get; }
    private protected MockAuthServiceClient MockAuthServiceClient { get; }
    private protected Guid PersonId { get; }

    protected SocialWorkerRegistrationPageTestBase()
    {
        PersonId = Guid.NewGuid();
        MockRegisterSocialWorkerJourneyService = new();
        MockAuthServiceClient = new();
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockRegisterSocialWorkerJourneyService.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
    }
}
