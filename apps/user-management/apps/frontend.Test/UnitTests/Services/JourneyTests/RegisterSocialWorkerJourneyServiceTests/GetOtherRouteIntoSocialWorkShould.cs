using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests;

public class GetOtherRouteIntoSocialWorkShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnOtherRouteIntoSocialWork()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountBuilder.Build();

        var expected = new RegisterSocialWorkerJourneyModel(account);

        MockAccountService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(account);

        // Act
        var response = await Sut.GetOtherRouteIntoSocialWorkAsync(id);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected.OtherRouteIntoSocialWork);

        MockAccountService.Verify(x => x.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
