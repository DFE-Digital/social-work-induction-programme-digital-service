using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class SetSocialWorkerDetails : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsUserDetails()
    {
        // Arrange
        var existingDetails = new SocialWorkerFaker().Generate();
        var expected = new SocialWorkerFaker().Generate();
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { SocialWorkerDetails = existingDetails }
        );

        // Act
        Sut.SetSocialWorkerDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.SocialWorkerDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsUserDetails()
    {
        // Arrange
        var expected = new SocialWorkerFaker().Generate();

        // Act
        Sut.SetSocialWorkerDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.SocialWorkerDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }
}
