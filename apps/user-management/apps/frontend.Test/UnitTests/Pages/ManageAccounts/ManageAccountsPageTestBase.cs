using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Repositories;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public abstract class ManageAccountsPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    private protected Mock<IAccountService> MockAccountService { get; }

    private protected InMemoryAccountRepository AccountRepository { get; }
    private protected AccountFaker AccountFaker { get; }

    private protected CreateAccountJourneyService CreateAccountJourneyService { get; }

    private protected EditAccountJourneyService EditAccountJourneyService { get; }

    private protected Mock<ISocialWorkEnglandService> MockSocialWorkEnglandService { get; }

    private protected Mock<INotificationServiceClient> MockNotificationServiceClient { get; }

    protected ManageAccountsPageTestBase()
    {
        AccountFaker = new AccountFaker();
        MockAccountService = new Mock<IAccountService>();
        AccountRepository = new InMemoryAccountRepository();
        AccountRepository.AddRange(AccountFaker.Generate(10));

        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };
        CreateAccountJourneyService = new CreateAccountJourneyService(
            httpContextAccessor,
            AccountRepository
        );
        EditAccountJourneyService = new EditAccountJourneyService(
            httpContextAccessor,
            AccountRepository
        );
        MockSocialWorkEnglandService = new();
        MockNotificationServiceClient = new();
    }
}
