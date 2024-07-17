using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public class SelectUserTypeShould : AccountsControllerTestBase
{
    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.SelectUserType();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        var requestHeader = viewResult!.ViewData["Referer"]?.ToString();
        requestHeader.Should().Be("test-referer");
    }

    [Fact]
    public void Post_WhenCalled_RedirectsToAddUserDetails()
    {
        // Act
        var selectUserTypeModel = new SelectUserTypeModel
        {
            AccountType = AccountType.Coordinator
        };
        var result = Sut.SelectUserType(selectUserTypeModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult!.ControllerName.Should().BeNull();
        redirectToActionResult.ActionName.Should().Be("AddUserDetails");
    }
}
