using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
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
            MockCreateAccountJourneyService.Object,
            new AccountDetailsValidator(),
            new FakeLinkGenerator()
        );
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Get_WhenCalled_LoadsTheView(bool isStaff)
    {
        // Arrange
        Sut.IsStaff = isStaff;
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(isStaff);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        // Assert
        Sut.FirstName.Should().Be(account.FirstName);
        Sut.MiddleNames.Should().Be(account.MiddleNames);
        Sut.LastName.Should().Be(account.LastName);
        Sut.Email.Should().Be(account.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(account.SocialWorkEnglandNumber);
        Sut.BackLinkPath.Should()
            .Be(
                isStaff
                    ? "/manage-accounts/select-use-case"
                    : "/manage-accounts/select-account-type"
            );

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithoutSocialWorkNumber_RedirectsToConfirmAccountDetails()
    {
        // Arrange
        var sweId = "1";
        var account = AccountBuilder
            .WithSocialWorkEnglandNumber(sweId)
            .WithTypes(ImmutableList.Create(AccountType.EarlyCareerSocialWorker))
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.FirstName = accountDetails.FirstName;
        Sut.MiddleNames = accountDetails.MiddleNames;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.SocialWorkEnglandNumber = sweId;

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);
        MockCreateAccountJourneyService.Setup(x =>
            x.SetAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails))
        );

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/social-worker-programme-dates");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateAccountJourneyService.Verify(
            x => x.SetAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidDataAndIsNotStaff_ReturnsErrorsAndRedirectsToAddAccountDetails()
    {
        // Arrange
        Sut.IsStaff = false;
        Sut.FirstName = string.Empty;
        Sut.LastName = string.Empty;
        Sut.Email = string.Empty;
        Sut.SocialWorkEnglandNumber = string.Empty;

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(4);
        modelStateKeys.Should().Contain("FirstName");
        modelState["FirstName"]!.Errors.Count.Should().Be(1);
        modelState["FirstName"]!.Errors[0].ErrorMessage.Should().Be("Enter a first name");

        modelStateKeys.Should().Contain("LastName");
        modelState["LastName"]!.Errors.Count.Should().Be(1);
        modelState["LastName"]!.Errors[0].ErrorMessage.Should().Be("Enter a last name");

        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");

        modelStateKeys.Should().Contain("SocialWorkEnglandNumber");
        modelState["SocialWorkEnglandNumber"]!.Errors.Count.Should().Be(1);
        modelState["SocialWorkEnglandNumber"]!.Errors[0].ErrorMessage.Should().Be("Enter a Social Work England registration number");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidDataAndIsStaff_ReturnsErrorsAndRedirectsToAddAccountDetails()
    {
        // Arrange
        Sut.IsStaff = true;
        Sut.FirstName = string.Empty;
        Sut.LastName = string.Empty;
        Sut.Email = string.Empty;
        Sut.SocialWorkEnglandNumber = string.Empty;

        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(3);
        modelStateKeys.Should().Contain("FirstName");
        modelState["FirstName"]!.Errors.Count.Should().Be(1);
        modelState["FirstName"]!.Errors[0].ErrorMessage.Should().Be("Enter a first name");

        modelStateKeys.Should().Contain("LastName");
        modelState["LastName"]!.Errors.Count.Should().Be(1);
        modelState["LastName"]!.Errors[0].ErrorMessage.Should().Be("Enter a last name");

        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
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
        // Act
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
    }
}
