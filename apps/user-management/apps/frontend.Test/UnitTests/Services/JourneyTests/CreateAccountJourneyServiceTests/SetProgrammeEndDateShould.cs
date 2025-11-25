using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetProgrammeEndDateShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsProgrammeEndDate()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { ProgrammeEndDate = expected}
        );

        // Act
        Sut.SetProgrammeEndDate(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.ProgrammeEndDate.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsProgrammeEndDate()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

        // Act
        Sut.SetProgrammeEndDate(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.ProgrammeEndDate.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
