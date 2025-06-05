using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class GetUserDetailsShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnUserDetails()
    {
        // Arrange
        var user = UserBuilder.Build();

        var expected = new EditUserJourneyModel(user);

        MockUserService.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var response = await Sut.GetUserDetailsAsync(user.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<UserDetails>();
        response.Should().BeEquivalentTo(expected.UserDetails);

        MockUserService.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
