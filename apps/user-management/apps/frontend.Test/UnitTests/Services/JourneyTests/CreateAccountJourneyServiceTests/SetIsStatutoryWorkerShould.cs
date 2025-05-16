using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetIsStatutoryWorkerShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhenCalled_WithExistingSessionData_SetsAccountDetails(bool? expected)
    {
        // Arrange
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsStatutoryWorker = new Faker().PickRandom(true, false) }
        );

        // Act
        Sut.SetIsStatutoryWorker(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsStatutoryWorker.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhenCalled_WithBlankSession_SetsAccountDetails(bool? expected)
    {
        // Act
        Sut.SetIsStatutoryWorker(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsStatutoryWorker.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
