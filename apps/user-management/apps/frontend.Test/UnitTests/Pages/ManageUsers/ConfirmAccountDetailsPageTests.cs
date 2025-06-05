using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class ConfirmUserDetailsShould : ManageUsersPageTestBase<ConfirmUserDetails>
{
    private ConfirmUserDetails Sut { get; }

    public ConfirmUserDetailsShould()
    {
        Sut = new ConfirmUserDetails(
            MockCreateUserJourneyService.Object,
            MockEditUserJourneyService.Object,
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
        var expectedUserDetails = UserDetailsFaker.Generate();

        MockCreateUserJourneyService
            .Setup(x => x.GetUserDetails())
            .Returns(expectedUserDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.FirstName.Should().Be(expectedUserDetails.FirstName);
        Sut.LastName.Should().Be(expectedUserDetails.LastName);
        Sut.Email.Should().Be(expectedUserDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(expectedUserDetails.SocialWorkEnglandNumber);

        Sut.IsUpdatingUser.Should().BeFalse();
        Sut.BackLinkPath.Should().Be("/manage-users/add-user-details");
        Sut.ChangeDetailsLink.Should().Be("/manage-users/add-user-details?handler=Change");

        MockCreateUserJourneyService.Verify(x => x.GetUserDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task GetUpdate_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var user = UserBuilder.Build();
        var updatedUserDetails = UserDetails.FromUser(UserBuilder.Build());

        MockEditUserJourneyService
            .Setup(x => x.GetUserDetailsAsync(user.Id))
            .ReturnsAsync(updatedUserDetails);

        // Act
        var result = await Sut.OnGetUpdateAsync(user.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(user.Id);
        Sut.FirstName.Should().Be(updatedUserDetails.FirstName);
        Sut.LastName.Should().Be(updatedUserDetails.LastName);
        Sut.Email.Should().Be(updatedUserDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(updatedUserDetails.SocialWorkEnglandNumber);

        Sut.IsUpdatingUser.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-users/edit-user-details/" + user.Id);
        Sut.ChangeDetailsLink.Should()
            .Be("/manage-users/edit-user-details/" + user.Id + "?handler=Change");

        MockEditUserJourneyService.Verify(x => x.GetUserDetailsAsync(user.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalled_CreatesUserAndSendsEmailToNewUserWithInvitationTokenLink()
    {
        // Arrange
        var user = UserBuilder.Build();
        var updatedUserDetails = UserDetails.FromUser(user);

        MockCreateUserJourneyService
            .Setup(x => x.GetUserDetails())
            .Returns(updatedUserDetails);

        MockCreateUserJourneyService.Setup(x => x.CompleteJourneyAsync());

        var createUserRequest = new CreateMoodleUserRequest
        {
            Username = updatedUserDetails.Email,
            Email = updatedUserDetails.Email,
            FirstName = updatedUserDetails.FirstName,
            LastName = updatedUserDetails.LastName
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
        response!.Url.Should().Be("/manage-users");

        var notificationType = (NotificationBannerType?)TempData["NotificationType"];
        notificationType.Should().Be(NotificationBannerType.Success);

        var notificationHeader = TempData["NotificationHeader"]?.ToString();
        notificationHeader.Should().Be("New user added");

        var notificationMessage = TempData["NotificationMessage"]?.ToString();
        notificationMessage.Should().Be($"An invitation to register has been sent to {updatedUserDetails.FullName}, {updatedUserDetails.Email}");

        MockCreateUserJourneyService.Verify(x => x.GetUserDetails(), Times.Once);
        MockCreateUserJourneyService.Verify(x => x.SetExternalUserId(1), Times.Once);
        MockCreateUserJourneyService.Verify(x => x.CompleteJourneyAsync(), Times.Once);
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

        MockEditUserJourneyService
            .Setup(x => x.GetUserDetailsAsync(invalidId))
            .ReturnsAsync((UserDetails?)null);

        // Act
        var result = await Sut.OnGetUpdateAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditUserJourneyService.Verify(
            x => x.GetUserDetailsAsync(invalidId),
            Times.Once()
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostUpdate_WhenCalled_UpdatesUserDetailsAndRedirectsToUsersIndex()
    {
        // Arrange
        var user = UserBuilder.Build();

        MockEditUserJourneyService
            .Setup(x => x.IsUserIdValidAsync(user.Id))
            .ReturnsAsync(true);
        MockEditUserJourneyService.Setup(x => x.CompleteJourneyAsync(user.Id));

        // Act
        var result = await Sut.OnPostUpdateAsync(user.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/view-user-details/" + user.Id);

        MockEditUserJourneyService.Verify(x => x.IsUserIdValidAsync(user.Id), Times.Once);
        MockEditUserJourneyService.Verify(x => x.CompleteJourneyAsync(user.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostUpdate_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        MockEditUserJourneyService.Setup(x => x.IsUserIdValidAsync(id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostUpdateAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditUserJourneyService.Verify(x => x.IsUserIdValidAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
