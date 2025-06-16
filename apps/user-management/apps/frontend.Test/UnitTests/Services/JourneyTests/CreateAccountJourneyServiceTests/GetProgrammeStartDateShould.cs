using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetProgrammeStartDateShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsProgrammeStartDate()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.Today);
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { ProgrammeStartDate = expected }
        );

        // Act
        var response = Sut.GetProgrammeStartDate();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetProgrammeStartDate();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
