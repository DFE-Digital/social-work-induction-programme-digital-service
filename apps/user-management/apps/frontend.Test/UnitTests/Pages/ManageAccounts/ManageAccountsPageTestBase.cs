using Dfe.Sww.Ecf.Frontend.Repositories;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Microsoft.AspNetCore.Http;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public abstract class ManageAccountsPageTestBase : PageModelTestBase
{
    private protected IAccountRepository AccountRepository { get; }

    private protected AccountFaker AccountFaker { get; }

    private protected CreateAccountJourneyService CreateAccountJourneyService { get; }

    protected ManageAccountsPageTestBase()
    {
        AccountFaker = new AccountFaker();
        AccountRepository = new InMemoryAccountRepository();
        AccountRepository.AddRange(AccountFaker.Generate(10));

        var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpContext };
        CreateAccountJourneyService = new CreateAccountJourneyService(
            httpContextAccessor,
            AccountRepository
        );
    }
}
