using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class GetIsStaffShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnIsStaff()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountFaker.Generate();

        var expected = new EditAccountJourneyModel(account);

        MockAccountService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(account);

        // Act
        var response = await Sut.GetIsStaffAsync(id);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected.IsStaff);

        MockAccountService.Verify(x => x.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
