using Dfe.Sww.Ecf.Frontend.Models;
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

        MockClient.Setup(x => x.Accounts.GetAllAsync()).ReturnsAsync(persons);

        // Act
        var response = await Sut.GetAllAsync();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<List<Account>>();
        response.Should().BeEquivalentTo(accounts);

        MockClient.Verify(x => x.Accounts.GetAllAsync(), Times.Once);
        MockClient.VerifyNoOtherCalls();
    }
}
