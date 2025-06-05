using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.UserServiceTests;

public class GetByIdShould : UserServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsMatchingUsers()
    {
        // Arrange
        var person = PersonFaker.Generate();
        var user = Mapper.MapToBo(person);

        MockClient.Setup(x => x.Users.GetByIdAsync(person.PersonId)).ReturnsAsync(person);

        // Act
        var response = await Sut.GetByIdAsync(person.PersonId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<User>();
        response.Should().BeEquivalentTo(user);

        MockClient.Verify(x => x.Users.GetByIdAsync(person.PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenNoPersonFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        MockClient.Setup(x => x.Users.GetByIdAsync(id)).ReturnsAsync((Person?)null);

        // Act
        var response = await Sut.GetByIdAsync(id);

        // Assert
        response.Should().BeNull();

        MockClient.Verify(x => x.Users.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
