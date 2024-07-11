using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public class IndexShould : AccountsControllerTestBase
{
    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Get_WhenUserIsAdded_DisplayAddedUserInView()
    {
        // Arrange
        var accounts = AccountsRepository.GetAll();
        var newAccount = AccountFaker.GenerateNewUser();
        newAccount.Id = accounts.Count;

        Sut.SelectUserType(newAccount.Types!.First());
        await Sut.AddUserDetails(newAccount);

        Sut.SelectUserType(newAccount.Types!.First());
        await Sut.AddUserDetails(newAccount);
        Sut.ConfirmUserDetails_Post();

        // Act
        var result = Sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var updatedAccounts = AccountsRepository.GetAll();
        updatedAccounts.Should().ContainEquivalentOf(newAccount);

        var viewResult = result as ViewResult;
        viewResult!.ViewData.Model.Should().BeEquivalentTo(updatedAccounts);
        viewResult.TempData["AccountAddedMessage"].Should().Be(newAccount.Email);
    }
}
