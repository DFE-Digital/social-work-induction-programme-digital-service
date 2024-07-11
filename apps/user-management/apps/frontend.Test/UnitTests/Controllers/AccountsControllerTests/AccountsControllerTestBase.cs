using Dfe.Sww.Ecf.Frontend.Controllers;
using Dfe.Sww.Ecf.Frontend.Models.DAL;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public abstract class AccountsControllerTestBase
{
    private protected AccountsRepository AccountsRepository { get; }

    private protected AccountFaker AccountFaker { get; }

    private protected AccountsController Sut { get; }

    protected AccountsControllerTestBase()
    {
        AccountFaker = new AccountFaker();
        AccountsRepository = new AccountsRepository();
        AccountsRepository.AddRange(AccountFaker.Generate(10));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Referer"] = "test-referer";

        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        Sut = new AccountsController(new AccountValidator(), AccountsRepository)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = tempData
        };
    }
}
