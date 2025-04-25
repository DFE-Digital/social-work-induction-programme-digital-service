using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class ConfirmAccountDetailsShould : ManageAccountsPageTestBase<ConfirmAccountDetails>
{
    private ConfirmAccountDetails Sut { get; }

    public ConfirmAccountDetailsShould()
    {
        Sut = new ConfirmAccountDetails(
            MockCreateAccountJourneyService.Object,
            MockEditAccountJourneyService.Object,
            MockMoodleServiceClient.Object,
            new FakeLinkGenerator()
        )
        {
            TempData = TempData
        };
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var expectedAccountDetails = AccountDetailsFaker.Generate();

        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountDetails())
            .Returns(expectedAccountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.FirstName.Should().Be(expectedAccountDetails.FirstName);
        Sut.LastName.Should().Be(expectedAccountDetails.LastName);
        Sut.Email.Should().Be(expectedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(expectedAccountDetails.SocialWorkEnglandNumber);

        Sut.IsUpdatingAccount.Should().BeFalse();
        Sut.BackLinkPath.Should().Be("/manage-accounts/add-account-details");
        Sut.ChangeDetailsLink.Should().Be("/manage-accounts/add-account-details?handler=Change");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task GetUpdate_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var updatedAccountDetails = AccountDetails.FromAccount(AccountBuilder.Build());

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(updatedAccountDetails);

        // Act
        var result = await Sut.OnGetUpdateAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.FirstName.Should().Be(updatedAccountDetails.FirstName);
        Sut.LastName.Should().Be(updatedAccountDetails.LastName);
        Sut.Email.Should().Be(updatedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(updatedAccountDetails.SocialWorkEnglandNumber);

        Sut.IsUpdatingAccount.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-accounts/edit-account-details/" + account.Id);
        Sut.ChangeDetailsLink.Should()
            .Be("/manage-accounts/edit-account-details/" + account.Id + "?handler=Change");

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalled_CreatesAccountAndSendsEmailToNewAccountWithInvitationTokenLink()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var updatedAccountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountDetails())
            .Returns(updatedAccountDetails);

        MockCreateAccountJourneyService.Setup(x => x.CompleteJourneyAsync());

        var createUserRequest = new CreateMoodleUserRequest
        {
            Username = updatedAccountDetails.Email,
            Email = updatedAccountDetails.Email,
            FirstName = updatedAccountDetails.FirstName,
            LastName = updatedAccountDetails.LastName
        };
        MockMoodleServiceClient
            .Setup(x => x.User.CreateUserAsync(MoqHelpers.ShouldBeEquivalentTo(createUserRequest)))
            .ReturnsAsync(
                new CreateMoodleUserResponse
                {
                    Id = 1,
                    Username = "test",
                    Successful = true
                }
            );

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("/manage-accounts");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.CompleteJourneyAsync(), Times.Once);
        MockMoodleServiceClient.Verify(
            x => x.User.CreateUserAsync(MoqHelpers.ShouldBeEquivalentTo(createUserRequest)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task GetUpdate_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(invalidId))
            .ReturnsAsync((AccountDetails?)null);

        // Act
        var result = await Sut.OnGetUpdateAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(
            x => x.GetAccountDetailsAsync(invalidId),
            Times.Once()
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostUpdate_WhenCalled_UpdatesAccountDetailsAndRedirectsToAccountsIndex()
    {
        // Arrange
        var account = AccountBuilder.Build();

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(account.Id))
            .ReturnsAsync(true);
        MockEditAccountJourneyService.Setup(x => x.CompleteJourneyAsync(account.Id));

        // Act
        var result = await Sut.OnPostUpdateAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.CompleteJourneyAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostUpdate_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValidAsync(id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostUpdateAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
