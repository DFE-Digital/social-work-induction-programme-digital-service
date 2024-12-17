using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetSocialWorkerDetails : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsAccountDetails()
    {
        // Arrange
        var existingDetails = new SocialWorkerFaker().Generate();
        var expected = new SocialWorkerFaker().Generate();
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { SocialWorkerDetails = existingDetails }
        );

        // Act
        Sut.SetSocialWorkerDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.SocialWorkerDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsAccountDetails()
    {
        // Arrange
        var expected = new SocialWorkerFaker().Generate();

        // Act
        Sut.SetSocialWorkerDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.SocialWorkerDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }
}
