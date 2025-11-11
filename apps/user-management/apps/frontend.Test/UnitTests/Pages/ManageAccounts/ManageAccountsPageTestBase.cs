using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
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
    private protected Mock<ISocialWorkEnglandService> MockSocialWorkEnglandService { get; } = new();
    private protected Mock<IAccountService> MockAccountService { get; } = new();
    private protected Mock<IOrganisationService> MockOrganisationService { get; } = new();
    private protected Mock<IEmailService> MockEmailService { get; } = new();
    private protected Mock<IAuthServiceClient> MockAuthServiceClient { get; } = new();
    private protected Mock<IMoodleService> MockMoodleService { get; } = new();


    private protected Mock<AccountDetailsValidator> MockAccountDetailsValidator { get; } = new();

    private protected Mock<IOptions<FeatureFlags>> MockFeatureFlags { get; } = new();

    private protected void VerifyAllNoOtherCalls()
    {
        MockCreateAccountJourneyService.VerifyNoOtherCalls();
        MockEditAccountJourneyService.VerifyNoOtherCalls();
        MockSocialWorkEnglandService.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
        MockOrganisationService.VerifyNoOtherCalls();
        MockEmailService.VerifyNoOtherCalls();
        MockAuthServiceClient.VerifyNoOtherCalls();
        MockMoodleService.VerifyNoOtherCalls();
    }
}
