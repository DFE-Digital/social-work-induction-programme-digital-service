using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.UserServiceTests;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.UserServiceTests;

public class GetAllShould : UserServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnAllUsers()
    {
        // Arrange
        var persons = PersonFaker.Generate(10);
        var users = persons.Select(x => Mapper.MapToBo(x));

        var paginationRequest = new PaginationRequest(0, 10);
        var metaData = new PaginationMetaData
        {
            Page = 1,
            PageSize = 5,
            PageCount = 2,
            TotalCount = 10,
            Links = new Dictionary<string, MetaDataLink>()
        };
        var clientResponse = new PaginationResult<Person>
        {
            Records = persons,
            MetaData = metaData
        };
        var paginationResponse = new PaginationResult<User>
        {
            Records = users,
            MetaData = metaData
        };

        MockClient.SetupMockHttpContextAccessorWithOrganisationId();

        MockClient
            .Setup(x => x.Users.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)))
            .ReturnsAsync(clientResponse);

        // Act
        var response = await Sut.GetAllAsync(paginationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<PaginationResult<User>>();
        response.Should().BeEquivalentTo(paginationResponse);

        MockClient.Verify(
            x => x.Users.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledAndOrganisationClaimIsMissing_ThrowNullReferenceException()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(0, 10);

        MockClient.SetupMockHttpContextAccessorWithEmptyClaimsPrincipal();

        // Act & Assert
        await FluentActions.Awaiting(() => Sut.GetAllAsync(paginationRequest))
            .Should().ThrowAsync<NullReferenceException>();
    }
}
