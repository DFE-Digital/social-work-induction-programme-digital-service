using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using JetBrains.Annotations;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public abstract class ManageAccountsPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : ManageAccountsBasePageModel
{
    private protected AccountBuilder AccountBuilder { get; }
    private protected OrganisationBuilder OrganisationBuilder { get; }
    private protected AccountDetailsFaker AccountDetailsFaker { get; }
    private protected SocialWorkerFaker SocialWorkerFaker { get; }

    private protected Mock<ICreateAccountJourneyService> MockCreateAccountJourneyService { get; }

    private protected Mock<IEditAccountJourneyService> MockEditAccountJourneyService { get; }

    private protected Mock<ISocialWorkEnglandService> MockSocialWorkEnglandService { get; }
    private protected Mock<IAccountService> MockAccountService { get; }
    private protected Mock<IOrganisationService> MockOrganisationService { get; }
    private protected Mock<IAuthServiceClient> MockAuthServiceClient { get; }

    private protected Mock<IMoodleServiceClient> MockMoodleServiceClient { get; }

    protected ManageAccountsPageTestBase()
    {
        AccountBuilder = new();
        OrganisationBuilder = new();
        AccountDetailsFaker = new AccountDetailsFaker();
        SocialWorkerFaker = new SocialWorkerFaker();

        MockCreateAccountJourneyService = new();
        MockEditAccountJourneyService = new();
        MockSocialWorkEnglandService = new();
        MockAccountService = new();
        MockOrganisationService = new();
        MockAuthServiceClient = new();
        MockMoodleServiceClient = new();
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockCreateAccountJourneyService.VerifyNoOtherCalls();
        MockEditAccountJourneyService.VerifyNoOtherCalls();
        MockSocialWorkEnglandService.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
        MockOrganisationService.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
        MockMoodleServiceClient.VerifyNoOtherCalls();
    }
}
