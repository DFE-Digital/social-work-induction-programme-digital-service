using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class SetIsStatutoryWorkerShould : CreateUserJourneyServiceTestBase
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhenCalled_WithExistingSessionData_SetsUserDetails(bool? expected)
    {
        // Arrange
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { IsStatutoryWorker = new Faker().PickRandom(true, false) }
        );

        // Act
        Sut.SetIsStatutoryWorker(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.IsStatutoryWorker.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhenCalled_WithBlankSession_SetsUserDetails(bool? expected)
    {
        // Act
        Sut.SetIsStatutoryWorker(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.IsStatutoryWorker.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
