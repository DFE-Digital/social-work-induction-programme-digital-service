using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class ViewUserDetailsPageTests : ManageUsersPageTestBase<ViewAccountDetails>
{
    private ViewAccountDetails Sut { get; }

    public ViewUserDetailsPageTests()
    {
        Sut = new ViewAccountDetails(MockAccountService.Object, new FakeLinkGenerator())
        {
            TempData = TempData
        };
    }

    [Fact]
    public async Task Get_WhenCalledWithId_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Account.Should().BeEquivalentTo(account);

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        MockAccountService.VerifyNoOtherCalls();
    }
}
