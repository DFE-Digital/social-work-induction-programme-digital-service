using Dfe.Sww.Ecf.Frontend.Controllers;
using Dfe.Sww.Ecf.Frontend.Repositories;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public abstract class AccountsControllerTestBase
{
    private protected IAccountRepository AccountRepository { get; }

    private protected AccountFaker AccountFaker { get; }

    private protected CreateAccountJourneyService CreateAccountJourneyService { get; }

    private protected AccountsController Sut { get; }

    protected AccountsControllerTestBase()
    {
        AccountFaker = new AccountFaker();
        AccountRepository = new AccountRepository();
        AccountRepository.AddRange(AccountFaker.Generate(10));

        var httpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        CreateAccountJourneyService = new CreateAccountJourneyService(httpContextAccessor, AccountRepository);

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        Sut = new AccountsController(new AddUserDetailsModelValidator(), AccountRepository, CreateAccountJourneyService)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = tempData
        };
    }
}
