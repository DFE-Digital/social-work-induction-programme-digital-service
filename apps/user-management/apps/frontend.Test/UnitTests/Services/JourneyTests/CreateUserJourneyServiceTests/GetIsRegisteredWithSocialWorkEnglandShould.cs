using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class GetIsRegisteredWithSocialWorkEnglandShould : CreateUserJourneyServiceTestBase
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhenCalled_WithExistingSessionData_ReturnsUserTypes(bool? expected)
    {
        // Arrange
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { IsRegisteredWithSocialWorkEngland = expected }
        );

        // Act
        var response = Sut.GetIsRegisteredWithSocialWorkEngland();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetIsRegisteredWithSocialWorkEngland();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
