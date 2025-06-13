using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetProgrammeStartDateShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsProgrammeStartDate()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.Today);
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { ProgrammeStartDate = expected}
        );

        // Act
        Sut.SetProgrammeStartDate(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.ProgrammeStartDate.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsProgrammeStartDate()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.Today);

        // Act
        Sut.SetProgrammeStartDate(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.ProgrammeStartDate.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
