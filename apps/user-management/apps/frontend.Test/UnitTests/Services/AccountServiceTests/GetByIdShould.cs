using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public class GetByIdShould : AccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsMatchingAccount()
    {
        // Arrange
        var person = PersonFaker.Generate();
        var account = Mapper.MapToBo(person);

        MockClient.Setup(x => x.Accounts.GetByIdAsync(person.PersonId)).ReturnsAsync(person);

        // Act
        var response = await Sut.GetByIdAsync(person.PersonId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Account>();
        response.Should().BeEquivalentTo(account);

        MockClient.Verify(x => x.Accounts.GetByIdAsync(person.PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenNoPersonFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockClient.Setup(x => x.Accounts.GetByIdAsync(id)).ReturnsAsync((Person?)null);

        // Act
        var response = await Sut.GetByIdAsync(id);

        // Assert
        response.Should().BeNull();

        MockClient.Verify(x => x.Accounts.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
