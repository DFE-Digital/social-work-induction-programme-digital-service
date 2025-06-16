using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests.SelectEthnicGroupService;

public class GetOtherEthnicGroupOtherShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnOtherEthnicGroupOther()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountBuilder.Build();

        var expected = new RegisterSocialWorkerJourneyModel(account);

        MockAccountService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(account);

        // Act
        var response = await Sut.EthnicGroupService.GetOtherEthnicGroupOtherAsync(id);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected.OtherEthnicGroupOther);

        MockAccountService.Verify(x => x.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
