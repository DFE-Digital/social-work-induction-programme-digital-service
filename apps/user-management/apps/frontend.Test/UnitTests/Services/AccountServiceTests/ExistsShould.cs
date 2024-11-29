using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public class ExistsShould : AccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsExpectedResult()
    {
        // Arrange
        var person = PersonFaker.Generate();

        MockClient.Setup(x => x.Accounts.GetByIdAsync(person.PersonId)).ReturnsAsync((Person?)null);

        // Act
        var response = await Sut.ExistsAsync(person.PersonId);

        // Assert
        response.Should().BeFalse();

        MockClient.Verify(x => x.Accounts.GetByIdAsync(person.PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
