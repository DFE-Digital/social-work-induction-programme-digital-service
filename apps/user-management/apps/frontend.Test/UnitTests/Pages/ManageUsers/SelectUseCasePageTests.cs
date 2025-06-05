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

public class SelectUseCasePageTests : ManageUsersPageTestBase<SelectUseCase>
{
    private SelectUseCase Sut { get; }

    public SelectUseCasePageTests()
    {
        Sut = new SelectUseCase(
            MockCreateUserJourneyService.Object,
            new SelectUseCaseValidator(),
            new FakeLinkGenerator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.SelectedUserTypes.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-users/select-user-type");

        MockCreateUserJourneyService.Verify(x => x.GetUserTypes(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithNullSelectedUserTypes_ReturnsErrorsAndRedirectsToSelectUserType()
    {
        // Arrange
        Sut.SelectedUserTypes = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("SelectedUserTypes");
        modelState["SelectedUserTypes"]!.Errors.Count.Should().Be(1);
        modelState["SelectedUserTypes"]!.Errors[0].ErrorMessage.Should().Be("Select what the user needs to do");

        Sut.BackLinkPath.Should().Be("/manage-users/select-user-type");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenSelectedUserTypesIsPopulated_RedirectsToAddUserDetails()
    {
        // Arrange
        Sut.SelectedUserTypes = new List<UserType> { UserType.Assessor };

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/add-user-details");

        MockCreateUserJourneyService.Verify(x => x.SetUserTypes(It.IsAny<List<UserType>>()), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
