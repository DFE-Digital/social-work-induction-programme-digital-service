using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.NameMatch.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.SocialWorkEnglandServiceTests;

public class GetByIdShould : SocialWorkEnglandTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnSocialWorker()
    {
        // Arrange
        var id = 1;
        var socialWorker = SocialWorkerFaker.GenerateWithId(id);

        MockClient.Setup(x => x.SocialWorkers.GetByIdAsync(id)).ReturnsAsync(socialWorker);

        // Act
        var response = await Sut.GetById(id.ToString());

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(socialWorker);

        MockClient.Verify(x => x.SocialWorkers.GetByIdAsync(id), Times.Once);
        MockClient.VerifyNoOtherCalls();

        MockSocialWorkerValidatorService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledWithNonNumericString_ReturnsNull()
    {
        // Arrange
        var id = "NOT AN ID";

        // Act
        var response = await Sut.GetById(id);

        // Assert
        response.Should().BeNull();

        MockClient.VerifyNoOtherCalls();
        MockSocialWorkerValidatorService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledWithNullId_ReturnsNull()
    {
        // Act
        var response = await Sut.GetById(null);

        // Assert
        response.Should().BeNull();

        MockClient.VerifyNoOtherCalls();
        MockSocialWorkerValidatorService.VerifyNoOtherCalls();
    }
}
