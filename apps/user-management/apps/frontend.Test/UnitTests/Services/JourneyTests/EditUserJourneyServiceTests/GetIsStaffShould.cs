using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditUserJourneyServiceTests;

public class GetIsStaffShould : EditUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnIsStaff()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserBuilder.Build();

        var expected = new EditUserJourneyModel(user);

        MockUserService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(user);

        // Act
        var response = await Sut.GetIsStaffAsync(id);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected.IsStaff);

        MockUserService.Verify(x => x.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
