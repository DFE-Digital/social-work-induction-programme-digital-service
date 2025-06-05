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

public class SelectUserTypePageTests : ManageUsersPageTestBase<SelectUserType>
{
    private SelectUserType Sut { get; }

    public SelectUserTypePageTests()
    {
        Sut = new SelectUserType(
            MockCreateUserJourneyService.Object,
            new FakeLinkGenerator(),
            new SelectUserTypeValidator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.IsStaff.Should().BeNull();
        Sut.EditUserId.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-users");

        MockCreateUserJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetNew_WhenCalled_ResetsModelAndRedirectsToSelectUserType()
    {
        // Act
        var result = Sut.OnGetNew();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be("/manage-users/select-user-type");

        MockCreateUserJourneyService.Verify(x => x.ResetCreateUserJourneyModel(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithNullIsStaff_ReturnsErrorsAndRedirectsToSelectUserType()
    {
        // Arrange
        Sut.IsStaff = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsStaff");
        modelState["IsStaff"]!.Errors.Count.Should().Be(1);
        modelState["IsStaff"]!.Errors[0].ErrorMessage.Should().Be("Select the type of user you want to add");

        Sut.BackLinkPath.Should().Be("/manage-users");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIsStaffTrue_RedirectsToSelectUseCase()
    {
        // Arrange
        Sut.IsStaff = true;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/select-use-case");

        MockCreateUserJourneyService.Verify(x => x.SetIsStaff(true), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIsStaffFalse_RedirectsToEligibilityInformation()
    {
        // Arrange
        Sut.IsStaff = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/eligibility-information");

        MockCreateUserJourneyService.Verify(x => x.SetIsStaff(false), Times.Once);
        MockCreateUserJourneyService.Verify(x => x.SetUserTypes(new List<UserType>
            { UserType.EarlyCareerSocialWorker }), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
