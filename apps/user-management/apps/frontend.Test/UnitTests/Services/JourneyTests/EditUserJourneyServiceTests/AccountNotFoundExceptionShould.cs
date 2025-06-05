using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class UserNotFoundExceptionShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ThrowsException()
    {
        // Arrange
        var user = UserBuilder.Build();

        var expectedException = new KeyNotFoundException("User not found with ID " + user.Id);

        MockUserService.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync((User?)null);

        // Act
        var actualException = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => Sut.SetIsStaffAsync(user.Id, false)
        );

        // Assert
        actualException.Message.Should().Be(expectedException.Message);

        MockUserService.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
