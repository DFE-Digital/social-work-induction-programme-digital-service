using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetProgrammeEndDateShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsProgrammeEndDate()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { ProgrammeEndDate = expected }
        );

        // Act
        var response = Sut.GetProgrammeEndDate();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetProgrammeEndDate();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
