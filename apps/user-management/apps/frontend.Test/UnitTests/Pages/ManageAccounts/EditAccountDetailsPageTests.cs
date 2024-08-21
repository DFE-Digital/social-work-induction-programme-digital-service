using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EditAccountDetailsPageTests : ManageAccountsPageTestBase<EditAccountDetails>
{
    private EditAccountDetails Sut { get; }

    public EditAccountDetailsPageTests()
    {
        Sut = new EditAccountDetails(
            EditAccountJourneyService,
            new AccountDetailsValidator(),
            new FakeLinkGenerator()
        )
        {
            TempData = TempData
        };
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheViewWithAccountDetails()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();

        // Act
        var result = Sut.OnGet(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.FirstName.Should().Be(account.FirstName);
        Sut.LastName.Should().Be(account.LastName);
        Sut.Email.Should().Be(account.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(account.SocialWorkEnglandNumber);
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);
    }

    [Fact]
    public void Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = Sut.OnGet(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();

        // Act
        var result = Sut.OnGetChange(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.FirstName.Should().Be(account.FirstName);
        Sut.LastName.Should().Be(account.LastName);
        Sut.Email.Should().Be(account.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(account.SocialWorkEnglandNumber);
    }

    [Fact]
    public void GetChange_WhenCalled_HasCorrectBackLink()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();

        // Act
        _ = Sut.OnGetChange(account.Id);

        // Assert
        Sut.BackLinkPath.Should()
            .Be("/manage-accounts/confirm-account-details/" + account.Id + "?handler=Update");
    }

    [Fact]
    public void GetChange_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = Sut.OnGetChange(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Post_WhenCalled_RedirectsToConfirmAccountDetails()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        Sut.FirstName = account.FirstName;
        Sut.LastName = account.LastName;
        Sut.Email = account.Email;
        Sut.SocialWorkEnglandNumber = account.SocialWorkEnglandNumber;

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts/confirm-account-details/" + account.Id + "?handler=Update");
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidData_ReturnsErrorsAndLoadsTheView()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        Sut.FirstName = account.FirstName;
        Sut.LastName = account.LastName;
        Sut.Email = string.Empty;

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email");
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = await Sut.OnPostAsync(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostChange_WhenCalled_HasCorrectBackLink()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        Sut.FirstName = account.FirstName;
        Sut.LastName = account.LastName;
        Sut.Email = account.Email;
        Sut.SocialWorkEnglandNumber = account.SocialWorkEnglandNumber;

        // Act
        _ = await Sut.OnPostChangeAsync(account.Id);

        // Assert
        Sut.BackLinkPath.Should()
            .Be($"/manage-accounts/confirm-account-details/{account.Id}?handler=Update");
    }
}
