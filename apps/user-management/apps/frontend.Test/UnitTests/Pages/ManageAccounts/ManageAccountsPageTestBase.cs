using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
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
    private protected AccountBuilder AccountBuilder { get; } = new();
    private protected OrganisationBuilder OrganisationBuilder { get; } = new();
    private protected AccountDetailsFaker AccountDetailsFaker { get; } = new();

    private protected Mock<ICreateAccountJourneyService> MockCreateAccountJourneyService { get; } = new();
    private protected Mock<IEditAccountJourneyService> MockEditAccountJourneyService { get; } = new();
    private protected Mock<IAccountService> MockAccountService { get; } = new();
    private protected Mock<IOrganisationService> MockOrganisationService { get; } = new();
    private protected Mock<IAuthServiceClient> MockAuthServiceClient { get; } = new();

    private protected void VerifyAllNoOtherCalls()
    {
        MockCreateAccountJourneyService.VerifyNoOtherCalls();
        MockEditAccountJourneyService.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
        MockOrganisationService.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
        MockEmailService.VerifyNoOtherCalls();
    }
}
