using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class ViewUserDetailsPageTests : ManageUsersPageTestBase<ViewUserDetails>
{
    private ViewUserDetails Sut { get; }

    public ViewUserDetailsPageTests()
    {
        Sut = new ViewUserDetails(MockUserService.Object, new FakeLinkGenerator())
        {
            TempData = TempData
        };
    }

    [Fact]
    public async Task Get_WhenCalledWithId_LoadsTheView()
    {
        // Arrange
        var user = UserBuilder.Build();

        MockUserService.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await Sut.OnGetAsync(user.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.UserAccount.Should().BeEquivalentTo(user);

        MockUserService.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
        MockUserService.VerifyNoOtherCalls();
    }
}
