using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class AddAccountDetailsPageTests : ManageAccountsPageTestBase<AddAccountDetails>
{
    private AddAccountDetails Sut { get; }

    public AddAccountDetailsPageTests()
    {
        Sut = new AddAccountDetails(
            CreateAccountJourneyService,
            new AccountDetailsValidator(),
            new FakeLinkGenerator(),
            MockSocialWorkEnglandService.Object
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Get_WhenCalled_HasCorrectBackLink(bool isStaff)
    {
        // Arrange
        Sut.IsStaff = isStaff;

        // Act
        _ = Sut.OnGet();

        // Assert
        Sut.BackLinkPath.Should()
            .Be(
                isStaff
                    ? "/manage-accounts/select-use-case"
                    : "/manage-accounts/select-account-type"
            );
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
    public async Task Post_WhenCalledWithoutSocialWorkNumber_RedirectsToConfirmAccountDetails()
    {
        // Arrange
        var accountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.SocialWorkEnglandNumber = null;

        MockSocialWorkEnglandService
            .Setup(x => x.GetById(It.IsAny<string>()))
            .ReturnsAsync((SocialWorker?)null);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/confirm-account-details");

        MockSocialWorkEnglandService.Verify(x => x.GetById(It.IsAny<string>()), Times.Once);
        MockSocialWorkEnglandService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithSocialWorkerNumber_RedirectsToAddExistingUser()
    {
        // Arrange
        var accountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.SocialWorkEnglandNumber = "1";

        MockSocialWorkEnglandService
            .Setup(x => x.GetById(It.IsAny<string>()))
            .ReturnsAsync(new SocialWorker());

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/add-existing-user");

        MockSocialWorkEnglandService.Verify(x => x.GetById(It.IsAny<string>()), Times.Once);
        MockSocialWorkEnglandService.VerifyNoOtherCalls();
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

        MockSocialWorkEnglandService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenSocialWorkerReturnsNull_ReturnsErrors()
    {
        // Arrange
        var accountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        Sut.FirstName = accountDetails.FirstName;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.SocialWorkEnglandNumber = "123";

        MockSocialWorkEnglandService
            .Setup(x => x.GetById(It.IsAny<string>()))
            .ReturnsAsync((SocialWorker?)null);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain(nameof(Sut.SocialWorkEnglandNumber));
        modelState[nameof(Sut.SocialWorkEnglandNumber)]!.Errors.Count.Should().Be(1);
        modelState[nameof(Sut.SocialWorkEnglandNumber)]!
            .Errors[0]
            .ErrorMessage.Should()
            .Be("Failed to retrieve Social Work England record. Please try again later.");

        MockSocialWorkEnglandService.Verify(x => x.GetById(It.IsAny<string>()), Times.Once);
        MockSocialWorkEnglandService.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Post_WhenCalledWithInvalidData_HasCorrectBackLink(bool isStaff)
    {
        // Arrange
        Sut.IsStaff = isStaff;

        // Act
        _ = await Sut.OnPostAsync();

        // Assert
        Sut.BackLinkPath.Should()
            .Be(
                isStaff
                    ? "/manage-accounts/select-use-case"
                    : "/manage-accounts/select-account-type"
            );
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
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
    }
}
