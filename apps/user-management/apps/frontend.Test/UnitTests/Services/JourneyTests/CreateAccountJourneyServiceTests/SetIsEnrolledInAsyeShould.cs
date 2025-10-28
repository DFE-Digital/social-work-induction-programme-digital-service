using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetIsEnrolledInAsyeShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsIsEnrolledInAsye()
    {
        // Arrange
        var expected = true;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsEnrolledInAsye = expected }
        );

        // Act
        Sut.SetIsEnrolledInAsye(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsEnrolledInAsye.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsIsEnrolledInAsye()
    {
        // Arrange
        var expected = true;

        // Act
        Sut.SetIsEnrolledInAsye(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsEnrolledInAsye.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
