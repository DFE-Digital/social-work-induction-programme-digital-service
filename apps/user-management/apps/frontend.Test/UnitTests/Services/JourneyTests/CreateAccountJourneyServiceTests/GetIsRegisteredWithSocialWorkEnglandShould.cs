using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsRegisteredWithSocialWorkEnglandShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhenCalled_WithExistingSessionData_ReturnsAccountTypes(bool? expected)
    {
        // Arrange
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsRegisteredWithSocialWorkEngland = expected }
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
