using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class EditUserDetailsPageTests : ManageUsersPageTestBase<EditUserDetails>
{
    private EditUserDetails Sut { get; }

    public EditUserDetailsPageTests()
    {
        Sut = new EditUserDetails(
            MockEditUserJourneyService.Object,
            new UserDetailsValidator(),
            new FakeLinkGenerator()
        )
        {
            TempData = TempData
        };
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithUserDetails()
    {
        // Arrange
        var user = UserBuilder.Build();
        var userDetails = UserDetails.FromUser(user);

        var isSwe = SocialWorkEnglandRecord.TryParse(user.SocialWorkEnglandNumber, out var swe);
        var socialWorkerId = isSwe ? swe?.GetNumber().ToString() : null;

        MockEditUserJourneyService
            .Setup(x => x.GetUserDetailsAsync(user.Id))
            .ReturnsAsync(userDetails);

        MockEditUserJourneyService.Setup(x => x.GetIsStaffAsync(user.Id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnGetAsync(user.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(user.Id);
        Sut.FirstName.Should().Be(user.FirstName);
        Sut.LastName.Should().Be(user.LastName);
        Sut.Email.Should().Be(user.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(socialWorkerId);
        Sut.IsStaff.Should().Be(false);
        Sut.BackLinkPath.Should().Be("/manage-users/view-user-details/" + user.Id);

        MockEditUserJourneyService.Verify(x => x.GetUserDetailsAsync(user.Id), Times.Once);
        MockEditUserJourneyService.Verify(x => x.GetIsStaffAsync(user.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var id = Guid.NewGuid();

        MockEditUserJourneyService
            .Setup(x => x.GetUserDetailsAsync(id))
            .ReturnsAsync((UserDetails?)null);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditUserJourneyService.Verify(x => x.GetUserDetailsAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task GetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var user = UserBuilder.Build();
        var userDetails = UserDetails.FromUser(user);

        MockEditUserJourneyService
            .Setup(x => x.GetUserDetailsAsync(user.Id))
            .ReturnsAsync(userDetails);

        MockEditUserJourneyService.Setup(x => x.GetIsStaffAsync(user.Id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnGetChangeAsync(user.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(user.Id);
        Sut.FirstName.Should().Be(user.FirstName);
        Sut.LastName.Should().Be(user.LastName);
        Sut.Email.Should().Be(user.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(user.SocialWorkEnglandNumber);
        Sut.IsStaff.Should().Be(false);
        Sut.BackLinkPath.Should()
            .Be("/manage-users/confirm-user-details/" + user.Id + "?handler=Update");

        MockEditUserJourneyService.Verify(x => x.GetUserDetailsAsync(user.Id), Times.Once);
        MockEditUserJourneyService.Verify(x => x.GetIsStaffAsync(user.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task GetChange_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var id = Guid.NewGuid();

        MockEditUserJourneyService
            .Setup(x => x.GetUserDetailsAsync(id))
            .ReturnsAsync((UserDetails?)null);

        // Act
        var result = await Sut.OnGetChangeAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditUserJourneyService.Verify(x => x.GetUserDetailsAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalled_RedirectsToConfirmUserDetails()
    {
        // Arrange
        var user = UserBuilder.Build();
        var userDetails = UserDetails.FromUser(user);

        MockEditUserJourneyService
            .Setup(x => x.IsUserIdValidAsync(user.Id))
            .ReturnsAsync(true);
        MockEditUserJourneyService.Setup(x =>
            x.SetUserDetailsAsync(user.Id, MoqHelpers.ShouldBeEquivalentTo(userDetails))
        );

        Sut.FirstName = user.FirstName;
        Sut.LastName = user.LastName;
        Sut.Email = user.Email;
        Sut.SocialWorkEnglandNumber = user.SocialWorkEnglandNumber;
        Sut.IsStaff = user.IsStaff;

        // Act
        var result = await Sut.OnPostAsync(user.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-users/confirm-user-details/" + user.Id + "?handler=Update");

        MockEditUserJourneyService.Verify(x => x.IsUserIdValidAsync(user.Id), Times.Once);
        MockEditUserJourneyService.Verify(
            x =>
                x.SetUserDetailsAsync(
                    user.Id,
                    MoqHelpers.ShouldBeEquivalentTo(userDetails)
                ),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidData_ReturnsErrorsAndLoadsTheView()
    {
        // Arrange
        var user = UserBuilder.Build();

        Sut.FirstName = user.FirstName;
        Sut.LastName = user.LastName;
        Sut.Email = string.Empty;
        Sut.IsStaff = false;
        Sut.SocialWorkEnglandNumber = "123";

        MockEditUserJourneyService
            .Setup(x => x.IsUserIdValidAsync(user.Id))
            .ReturnsAsync(true);

        // Act
        var result = await Sut.OnPostAsync(user.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");
        Sut.BackLinkPath.Should().Be("/manage-users/view-user-details/" + user.Id);

        MockEditUserJourneyService.Verify(x => x.IsUserIdValidAsync(user.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockEditUserJourneyService.Setup(x => x.IsUserIdValidAsync(id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditUserJourneyService.Verify(x => x.IsUserIdValidAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostChange_WhenCalled_HasCorrectBackLink()
    {
        // Arrange
        var user = UserBuilder.Build();
        var userDetails = UserDetails.FromUser(user);

        Sut.FirstName = user.FirstName;
        Sut.LastName = user.LastName;
        Sut.Email = user.Email;
        Sut.SocialWorkEnglandNumber = user.SocialWorkEnglandNumber;
        Sut.IsStaff = user.IsStaff;

        MockEditUserJourneyService
            .Setup(x => x.IsUserIdValidAsync(user.Id))
            .ReturnsAsync(true);
        MockEditUserJourneyService.Setup(x =>
            x.SetUserDetailsAsync(user.Id, MoqHelpers.ShouldBeEquivalentTo(userDetails))
        );

        // Act
        _ = await Sut.OnPostChangeAsync(user.Id);

        // Assert
        Sut.BackLinkPath.Should()
            .Be($"/manage-users/confirm-user-details/{user.Id}?handler=Update");

        MockEditUserJourneyService.Verify(x => x.IsUserIdValidAsync(user.Id), Times.Once);
        MockEditUserJourneyService.Verify(
            x =>
                x.SetUserDetailsAsync(
                    user.Id,
                    MoqHelpers.ShouldBeEquivalentTo(userDetails)
                ),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }
}
