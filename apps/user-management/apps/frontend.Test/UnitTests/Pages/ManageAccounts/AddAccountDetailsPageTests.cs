using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class AddAccountDetailsPageTests : ManageAccountsPageTestBase
{
    private AddAccountDetails Sut { get; }

    public AddAccountDetailsPageTests()
    {
        Sut = new AddAccountDetails(
            CreateAccountJourneyService,
            new AccountDetailsValidator(),
            new FakeLinkGenerator()
        );
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void Get_WhenCalled_PopulatesModelFromJourneyState()
    {
        // Arrange
        var account = AccountFaker.GenerateNewAccount();
        CreateAccountJourneyService.PopulateJourneyModelFromAccount(account);

        // Act
        _ = Sut.OnGet();

        // Assert
        Sut.FirstName.Should().Be(account.FirstName);
        Sut.LastName.Should().Be(account.LastName);
        Sut.Email.Should().Be(account.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(account.SocialWorkEnglandNumber);
    }

    [Fact]
    public void GetChange_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void GetChange_WhenCalled_HasCorrectBackLink()
    {
        // Act
        _ = Sut.OnGetChange();

        // Assert
        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
    }

    [Fact]
    public async Task Post_WhenCalled_RedirectsToConfirmAccountDetails()
    {
        // Arrange
        var accountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.SocialWorkEnglandNumber = accountDetails.SocialWorkEnglandNumber;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/confirm-account-details");
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToAddAccountDetails()
    {
        // Arrange
        var accountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = string.Empty;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email");
    }
}
