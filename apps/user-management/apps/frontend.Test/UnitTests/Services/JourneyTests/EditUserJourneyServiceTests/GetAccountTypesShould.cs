using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class GetUserTypesShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnUserTypes()
    {
        // Arrange
        var user = UserBuilder.Build();

        var expected = new EditUserJourneyModel(user);

        MockUserService.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var response = await Sut.GetUserTypesAsync(user.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ImmutableList<UserType>?>();
        response.Should().BeEquivalentTo(expected.UserTypes);

        MockUserService.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
