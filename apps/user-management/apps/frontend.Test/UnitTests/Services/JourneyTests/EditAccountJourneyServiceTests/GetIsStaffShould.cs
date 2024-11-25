using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class GetIsStaffShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ReturnIsStaff()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountFaker.Generate();

        var expected = new EditAccountJourneyModel(account);

        MockAccountRepository.Setup(x => x.GetById(id)).Returns(account);

        // Act
        var response = Sut.GetIsStaff(id);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected.IsStaff);

        MockAccountRepository.Verify(x => x.GetById(id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
