using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsAgencyWorker : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsIsAgencyWorker()
    {
        // Arrange
        var expected = false;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsAgencyWorker = expected }
        );

        // Act
        var response = Sut.GetIsAgencyWorker();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetIsAgencyWorker();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
