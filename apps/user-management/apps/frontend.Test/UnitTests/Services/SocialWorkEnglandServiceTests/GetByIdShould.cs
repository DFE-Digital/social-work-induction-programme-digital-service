using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.SocialWorkEnglandServiceTests;

public class GetByIdShould
{
    private readonly SocialWorkerFaker _socialWorkerFaker;

    private readonly Mock<ISocialWorkEnglandClient> _mockClient;
    private readonly SocialWorkEnglandService _sut;

    public GetByIdShould()
    {
        _socialWorkerFaker = new();

        _mockClient = new();
        _sut = new(_mockClient.Object);
    }

    [Fact]
    public async Task WhenCalled_ReturnSocialWorker()
    {
        // Arrange
        var id = 1;
        var socialWorker = _socialWorkerFaker.GenerateWithId(id);

        _mockClient.Setup(x => x.SocialWorkers.GetByIdAsync(id)).ReturnsAsync(socialWorker);

        // Act
        var response = await _sut.GetSocialWorkerAsync(id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(socialWorker);

        _mockClient.Verify(x => x.SocialWorkers.GetByIdAsync(id), Times.Once);
        _mockClient.VerifyNoOtherCalls();
    }
}
