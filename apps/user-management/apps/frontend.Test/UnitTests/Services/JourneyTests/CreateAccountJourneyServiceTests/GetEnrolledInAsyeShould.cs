using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetEnrolledInAsyeShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsIsEnrolledInAsye()
    {
        // Arrange
        var expected = true;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsEnrolledInAsye = true }
        );

        // Act
        var response = Sut.GetIsEnrolledInAsye();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetIsEnrolledInAsye();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
