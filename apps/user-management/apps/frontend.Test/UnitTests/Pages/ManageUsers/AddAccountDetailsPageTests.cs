using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class AddUserDetailsPageTests : ManageUsersPageTestBase<AddUserDetails>
{
    private AddUserDetails Sut { get; }

    public AddUserDetailsPageTests()
    {
        Sut = new AddUserDetails(
            MockCreateUserJourneyService.Object,
            new UserDetailsValidator(),
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
        var user = UserBuilder.Build();
        var userDetails = UserDetails.FromUser(user);

        MockCreateUserJourneyService.Setup(x => x.GetIsStaff()).Returns(isStaff);
        MockCreateUserJourneyService.Setup(x => x.GetUserDetails()).Returns(userDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        // Assert
        Sut.FirstName.Should().Be(user.FirstName);
        Sut.LastName.Should().Be(user.LastName);
        Sut.Email.Should().Be(user.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(user.SocialWorkEnglandNumber);
        Sut.BackLinkPath.Should()
            .Be(
                isStaff
                    ? "/manage-users/select-use-case"
                    : "/manage-users/select-user-type"
            );

        MockCreateUserJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateUserJourneyService.Verify(x => x.GetUserDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var user = UserBuilder.Build();
        var userDetails = UserDetails.FromUser(user);

        MockCreateUserJourneyService.Setup(x => x.GetIsStaff()).Returns(false);
        MockCreateUserJourneyService.Setup(x => x.GetUserDetails()).Returns(userDetails);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-users/confirm-user-details");

        MockCreateUserJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateUserJourneyService.Verify(x => x.GetUserDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithoutSocialWorkNumber_RedirectsToConfirmUserDetails()
    {
        // Arrange
        var sweId = "1";
        var user = UserBuilder
            .WithSocialWorkEnglandNumber(sweId)
            .WithTypes(ImmutableList.Create(UserType.EarlyCareerSocialWorker))
            .Build();
        var userDetails = UserDetails.FromUser(user);

        Sut.FirstName = userDetails.FirstName;
        Sut.LastName = userDetails.LastName;
        Sut.Email = userDetails.Email;
        Sut.SocialWorkEnglandNumber = sweId;

        MockCreateUserJourneyService.Setup(x => x.GetIsStaff()).Returns(false);
        MockCreateUserJourneyService.Setup(x =>
            x.SetUserDetails(MoqHelpers.ShouldBeEquivalentTo(userDetails))
        );

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/social-worker-programme-dates");

        MockCreateUserJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        MockCreateUserJourneyService.Verify(
            x => x.SetUserDetails(MoqHelpers.ShouldBeEquivalentTo(userDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidDataAndIsNotStaff_ReturnsErrorsAndRedirectsToAddUserDetails()
    {
        // Arrange
        Sut.IsStaff = false;
        Sut.FirstName = string.Empty;
        Sut.LastName = string.Empty;
        Sut.Email = string.Empty;
        Sut.SocialWorkEnglandNumber = string.Empty;

        MockCreateUserJourneyService.Setup(x => x.GetIsStaff()).Returns(false);

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

        MockCreateUserJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidDataAndIsStaff_ReturnsErrorsAndRedirectsToAddUserDetails()
    {
        // Arrange
        Sut.IsStaff = true;
        Sut.FirstName = string.Empty;
        Sut.LastName = string.Empty;
        Sut.Email = string.Empty;
        Sut.SocialWorkEnglandNumber = string.Empty;

        MockCreateUserJourneyService.Setup(x => x.GetIsStaff()).Returns(false);

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

        MockCreateUserJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
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
                    ? "/manage-users/select-use-case"
                    : "/manage-users/select-user-type"
            );
    }

    [Fact]
    public async Task PostChange_WhenCalled_HasCorrectBackLink()
    {
        // Act
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.BackLinkPath.Should().Be("/manage-users/confirm-user-details");
    }
}
