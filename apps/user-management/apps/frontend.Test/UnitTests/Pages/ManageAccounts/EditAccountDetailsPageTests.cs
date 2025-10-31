using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using Person = Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Person;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EditAccountDetailsPageTests : ManageAccountsPageTestBase<EditAccountDetails>
{
    private EditAccountDetails Sut { get; }

    public EditAccountDetailsPageTests()
    {
        Sut = new EditAccountDetails(MockEditAccountJourneyService.Object, new TestAccountDetailsValidator(MockAccountService.Object, MockAuthServiceClient.Object),
            new FakeLinkGenerator())
        {
            TempData = TempData
        };
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithAccountDetails()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var expectedAccountType = accountDetails.Types ?? new List<AccountType>();

        var isSwe = SocialWorkEnglandRecord.TryParse(account.SocialWorkEnglandNumber, out var swe);
        var socialWorkerId = isSwe ? swe?.GetNumber().ToString() : null;

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(accountDetails);

        MockEditAccountJourneyService.Setup(x => x.GetAccountTypesAsync(account.Id)).ReturnsAsync(accountDetails.Types?.ToImmutableList());

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.FirstName.Should().Be(account.FirstName);
        Sut.LastName.Should().Be(account.LastName);
        Sut.Email.Should().Be(account.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(socialWorkerId);
        Sut.AccountTypes.Should().BeEquivalentTo(expectedAccountType);
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var id = Guid.NewGuid();

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(id))
            .ReturnsAsync((AccountDetails?)null);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalled_RedirectsToConfirmAccountDetails()
    {
        // Arrange
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var expectedAccountType = accountDetails.Types ?? new List<AccountType>();

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(account.Id))
            .ReturnsAsync(true);
        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountDetailsAsync(account.Id, MoqHelpers.ShouldBeEquivalentTo(accountDetails))
        );
        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(account.Id)).ReturnsAsync(accountDetails);

        Sut.FirstName = account.FirstName;
        Sut.MiddlesNames = account.MiddleNames;
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
        Sut.AccountTypes.Should().BeEquivalentTo(expectedAccountType);
        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(
            x =>
                x.SetAccountDetailsAsync(
                    account.Id,
                    MoqHelpers.ShouldBeEquivalentTo(accountDetails)
                ),
            Times.Once
        );
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidData_ReturnsErrorsAndLoadsTheView()
    {
        // Arrange
        var account = AccountBuilder
            .WithTypes([AccountType.Assessor])
            .WithSocialWorkEnglandNumber("SW321")
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var expectedAccountType = accountDetails.Types ?? new List<AccountType>();

        Sut.FirstName = account.FirstName;
        Sut.LastName = account.LastName;
        Sut.Email = string.Empty;
        Sut.AccountTypes = expectedAccountType;
        Sut.SocialWorkEnglandNumber = "SW123";

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(account.Id))
            .ReturnsAsync(true);
        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(account.Id)).ReturnsAsync(accountDetails);
        MockAuthServiceClient.Setup(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!)).ReturnsAsync((Person?)null);

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        MockAuthServiceClient.Verify(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(accountDetails.SocialWorkEnglandNumber!), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValidAsync(id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenEmailUnchanged_IgnoresUniquenessAndRedirects()
    {
        // Arrange
        var account = AccountBuilder.WithAddOrEditAccountDetailsData().Build();
        var accountDetails = AccountDetails.FromAccount(account);
        accountDetails.Email = "Test@Test.com";
        var postedEmail = "  test@test.com  ";

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(account.Id))
            .ReturnsAsync(true);
        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountDetailsAsync(account.Id, MoqHelpers.ShouldBeEquivalentTo(accountDetails))
        );
        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(account.Id)).ReturnsAsync(accountDetails);

        Sut.FirstName = account.FirstName;
        Sut.MiddlesNames = account.MiddleNames;
        Sut.LastName = account.LastName;
        Sut.Email = postedEmail;
        Sut.SocialWorkEnglandNumber = account.SocialWorkEnglandNumber;

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        MockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(It.IsAny<string>()), Times.Never);
        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.SetAccountDetailsAsync(account.Id, It.IsAny<AccountDetails>()), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenEmailChanged_ChecksUniqueness_AndReturnsErrorWhenNotUnique()
    {
        // Arrange
        var account = AccountBuilder.WithAddOrEditAccountDetailsData().Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var postedEmail = "different@test.com";

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValidAsync(account.Id)).ReturnsAsync(true);
        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(account.Id)).ReturnsAsync(accountDetails);
        MockAccountService.Setup(x => x.CheckEmailExistsAsync(postedEmail)).ReturnsAsync(true);

        Sut.FirstName = account.FirstName;
        Sut.MiddlesNames = account.MiddleNames;
        Sut.LastName = account.LastName;
        Sut.Email = postedEmail;
        Sut.SocialWorkEnglandNumber = account.SocialWorkEnglandNumber;

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("The email address entered belongs to an existing user");

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        MockAccountService.Verify(x => x.CheckEmailExistsAsync(postedEmail), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenAccountDetailsMissing_ReturnsBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValidAsync(id)).ReturnsAsync(true);
        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(id)).ReturnsAsync((AccountDetails?)null);

        // Act
        var result = await Sut.OnPostAsync(id);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
