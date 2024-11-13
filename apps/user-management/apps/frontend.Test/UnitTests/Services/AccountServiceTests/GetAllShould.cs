using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public class GetAllShould : AccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnAllAccounts()
    {
        // Arrange
        var persons = PersonFaker.Generate(10);
        var accounts = persons.Select(x => Mapper.MapToBo(x));

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
        var paginationResponse = new PaginationResult<Account>
        {
            Records = accounts,
            MetaData = metaData
        };

        MockClient
            .Setup(x => x.Accounts.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)))
            .ReturnsAsync(clientResponse);

        // Act
        var response = await Sut.GetAllAsync(paginationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<PaginationResult<Account>>();
        response.Should().BeEquivalentTo(paginationResponse);

        MockClient.Verify(
            x => x.Accounts.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)),
            Times.Once
        );
        MockClient.VerifyNoOtherCalls();
    }
}
