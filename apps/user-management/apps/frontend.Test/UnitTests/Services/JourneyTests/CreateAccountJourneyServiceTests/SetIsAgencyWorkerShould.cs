using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetIsAgencyWorkerShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsIsAgencyWorker()
    {
        // Arrange
        var expected = true;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsAgencyWorker = expected}
        );

        // Act
        Sut.SetIsAgencyWorker(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsAgencyWorker.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsIsAgencyWorker()
    {
        // Arrange
        var expected = true;

        // Act
        Sut.SetIsAgencyWorker(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsAgencyWorker.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
