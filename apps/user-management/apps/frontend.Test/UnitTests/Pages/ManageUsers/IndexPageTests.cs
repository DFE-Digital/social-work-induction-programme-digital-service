using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using ManageUsersIndex = Dfe.Sww.Ecf.Frontend.Pages.ManageUsers.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class IndexPageTests : ManageUsersPageTestBase<ManageUsersIndex>
{
    private ManageUsersIndex Sut { get; }

    public IndexPageTests()
    {
        Sut = new ManageUsersIndex(MockUserService.Object);
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithUsersSortedByCreatedAt()
    {
        // Arrange
        var expectedUsers = UserBuilder.BuildMany(10);

        var paginationRequest = new PaginationRequest(0, 10);
        var paginationResponse = new PaginationResult<User>
        {
            Records = expectedUsers,
            MetaData = new PaginationMetaData
            {
                Page = 1,
                PageSize = 5,
                PageCount = 2,
                TotalCount = 10,
                Links = new Dictionary<string, MetaDataLink>()
            }
        };

        MockUserService
            .Setup(x => x.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)))
            .ReturnsAsync(paginationResponse);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Users.Should().NotBeEmpty();
        Sut.Users.Should().BeEquivalentTo(expectedUsers);
        Sut.Users.Should().BeInAscendingOrder(x => x.CreatedAt);
    }
}
