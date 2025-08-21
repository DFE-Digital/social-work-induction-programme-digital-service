using System.Collections.Immutable;
using System.Globalization;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class ConfirmAccountDetailsShould : ManageAccountsPageTestBase<ConfirmAccountDetails>
{
    public ConfirmAccountDetailsShould()
    {
        MockFeatureFlags.SetupGet(x => x.Value).Returns(new FeatureFlags
        {
            EnableMoodleIntegration = true
        });
        Sut = new ConfirmAccountDetails(
            MockCreateAccountJourneyService.Object,
            MockEditAccountJourneyService.Object,
            MockMoodleServiceClient.Object,
            new FakeLinkGenerator(),
            MockFeatureFlags.Object
        )
        {
            TempData = TempData
        };
    }

    private ConfirmAccountDetails Sut { get; set; }

    [Fact]
    public void Get_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var expectedAccountDetails = AccountDetailsFaker.GenerateWithIsStaff(false);
        var expectedChangeLinks = new AccountChangeLinks();
        var expectedAccountTypes = ImmutableList.Create(AccountType.EarlyCareerSocialWorker);
        var expectedAccountLabels = new AccountLabels
        {
            IsStaffLabel = IsStaffLabels.IsStaffFalse,
            IsRegisteredWithSocialWorkEnglandLabel = "Yes",
            IsAgencyWorkerLabel = "No",
            IsStatutoryWorkerLabel = "Yes",
            IsRecentlyQualifiedLabel = "Yes"
        };
        var expectedStartDateDateOnly = DateOnly.FromDateTime(DateTime.Now);
        var expectedEndDateDateOnly = DateOnly.FromDateTime(DateTime.Now.AddYears(2));
        var expectedStartDate = expectedStartDateDateOnly.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        var expectedEndDate = expectedEndDateDateOnly.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountDetails())
            .Returns(expectedAccountDetails);
        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountChangeLinks(null))
            .Returns(expectedChangeLinks);
        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountTypes())
            .Returns(expectedAccountTypes);
        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountLabels())
            .Returns(expectedAccountLabels);
        MockCreateAccountJourneyService
            .Setup(x => x.GetProgrammeStartDate())
            .Returns(expectedStartDateDateOnly);
        MockCreateAccountJourneyService
            .Setup(x => x.GetProgrammeEndDate())
            .Returns(expectedEndDateDateOnly);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.FirstName.Should().Be(expectedAccountDetails.FirstName);
        Sut.LastName.Should().Be(expectedAccountDetails.LastName);
        Sut.MiddleNames.Should().Be(expectedAccountDetails.MiddleNames);
        Sut.Email.Should().Be(expectedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(expectedAccountDetails.SocialWorkEnglandNumber);
        Sut.IsStaff.Should().Be(expectedAccountDetails.IsStaff);
        Sut.IsUpdatingAccount.Should().BeFalse();
        Sut.BackLinkPath.Should().Be("/manage-accounts/social-worker-programme-dates");
        Sut.ChangeDetailsLinks.Should().BeEquivalentTo(expectedChangeLinks);
        Sut.AccountTypes.Should().BeEquivalentTo(expectedAccountTypes);
        Sut.UserType.Should().Be(expectedAccountLabels.IsStaffLabel);
        Sut.RegisteredWithSocialWorkEngland.Should().Be(expectedAccountLabels.IsRegisteredWithSocialWorkEnglandLabel);
        Sut.StatutoryWorker.Should().Be(expectedAccountLabels.IsStatutoryWorkerLabel);
        Sut.AgencyWorker.Should().Be(expectedAccountLabels.IsAgencyWorkerLabel);
        Sut.Qualified.Should().Be(expectedAccountLabels.IsRecentlyQualifiedLabel);
        Sut.IsStaff.Should().Be(expectedAccountDetails.IsStaff);
        Sut.ProgrammeStartDate.Should().Be(expectedStartDate);
        Sut.ProgrammeEndDate.Should().Be(expectedEndDate);

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeStartDate(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeEndDate(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountLabels(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountChangeLinks(null), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task GetUpdate_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var updatedAccountDetails = AccountDetails.FromAccount(AccountBuilder.Build());
        var expectedChangeLinks = new AccountChangeLinks
        {
            AccountTypesChangeLink = "/manage-accounts/select-use-case?handler=Change",
            FirstNameChangeLink = "/manage-accounts/add-account-details?handler=Change#FirstName",
            MiddleNamesChangeLink = "/manage-accounts/add-account-details?handler=Change#MiddleNames",
            LastNameChangeLink = "/manage-accounts/add-account-details?handler=Change#Lastname",
            EmailChangeLink = "/manage-accounts/add-account-details?handler=Change#Email",
            SocialWorkEnglandNumberChangeLink = "/manage-accounts/add-account-details?handler=Change#SocialWorkEnglandNumber",
            ProgrammeDatesChangeLink = "/manage-accounts/social-worker-programme-dates"
        };

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(updatedAccountDetails);

        MockEditAccountJourneyService.Setup(x => x.GetAccountChangeLinks(account.Id, null)).Returns(expectedChangeLinks);

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
        Sut.ChangeDetailsLinks.Should().Be(expectedChangeLinks);

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountChangeLinks(account.Id, null), Times.Once);
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

        MockCreateAccountJourneyService.Setup(x => x.CompleteJourneyAsync(null));

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

        TempData["NotificationType"].Should().Be(NotificationBannerType.Success);
        TempData["NotificationHeader"].Should().Be("New user added");
        TempData["NotificationMessage"].Should().Be($"An invitation to register has been sent to {updatedAccountDetails.FullName}, {updatedAccountDetails.Email}");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetExternalUserId(1), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.CompleteJourneyAsync(null), Times.Once);
        MockMoodleServiceClient.Verify(
            x => x.User.CreateUserAsync(MoqHelpers.ShouldBeEquivalentTo(createUserRequest)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithMoodleIntegrationDisabled_CreatesUserAndSendsEmailAndDoesNotCallMoodleAndSetExternalUserId()
    {
        MockFeatureFlags.SetupGet(x => x.Value).Returns(new FeatureFlags
        {
            EnableMoodleIntegration = false
        });
        // Arrange
        Sut = new ConfirmAccountDetails(
            MockCreateAccountJourneyService.Object,
            MockEditAccountJourneyService.Object,
            MockMoodleServiceClient.Object,
            new FakeLinkGenerator(),
            MockFeatureFlags.Object
        )
        {
            TempData = TempData
        };
        var account = AccountBuilder.Build();
        var updatedAccountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountDetails())
            .Returns(updatedAccountDetails);

        MockCreateAccountJourneyService.Setup(x => x.CompleteJourneyAsync(null));

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("/manage-accounts");

        TempData["NotificationType"].Should().Be(NotificationBannerType.Success);
        TempData["NotificationHeader"].Should().Be("New user added");
        TempData["NotificationMessage"].Should().Be($"An invitation to register has been sent to {updatedAccountDetails.FullName}, {updatedAccountDetails.Email}");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.CompleteJourneyAsync(null), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetExternalUserId(It.IsAny<int>()), Times.Never);
        MockMoodleServiceClient.VerifyNoOtherCalls();
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
        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(account.Id)).ReturnsAsync(AccountDetails.FromAccount(account));

        // Act
        var result = await Sut.OnPostUpdateAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        TempData["NotificationType"].Should().Be(NotificationBannerType.Success);
        TempData["NotificationHeader"].Should().Be("User details updated");
        TempData["NotificationMessage"].Should().Be($"An email has been sent to {account.FullName}, {account.Email}");

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.CompleteJourneyAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
