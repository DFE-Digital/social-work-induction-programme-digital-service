using SocialWorkInductionProgramme.Frontend.Services.Interfaces;
using SocialWorkInductionProgramme.Frontend.Services.Journeys.Interfaces;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Builders;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Fakers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Pages.ManageAccounts;

public abstract class ManageAccountsPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    private protected AccountBuilder AccountBuilder { get; }
    private protected AccountDetailsFaker AccountDetailsFaker { get; }

    private protected SocialWorkerFaker SocialWorkerFaker { get; }

    private protected Mock<ICreateAccountJourneyService> MockCreateAccountJourneyService { get; }

    private protected Mock<IEditAccountJourneyService> MockEditAccountJourneyService { get; }

    private protected Mock<ISocialWorkEnglandService> MockSocialWorkEnglandService { get; }
    private protected Mock<IAccountService> MockAccountService { get; }

    protected ManageAccountsPageTestBase()
    {
        AccountBuilder = new();
        AccountDetailsFaker = new AccountDetailsFaker();
        SocialWorkerFaker = new SocialWorkerFaker();

        MockCreateAccountJourneyService = new();
        MockEditAccountJourneyService = new();
        MockSocialWorkEnglandService = new();
        MockAccountService = new();
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockCreateAccountJourneyService.VerifyNoOtherCalls();
        MockEditAccountJourneyService.VerifyNoOtherCalls();
        MockSocialWorkEnglandService.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
    }
}
