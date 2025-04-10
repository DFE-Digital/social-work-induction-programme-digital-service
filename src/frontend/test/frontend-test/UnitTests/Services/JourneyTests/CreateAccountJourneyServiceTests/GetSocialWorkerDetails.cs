using SocialWorkInductionProgramme.Frontend.Extensions;
using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Models;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Xunit;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetSocialWorkerDetails : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsAccountDetails()
    {
        // Arrange
        var expected = new SocialWorkerFaker().Generate();
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { SocialWorkerDetails = expected }
        );

        // Act
        var response = Sut.GetSocialWorkerDetails();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetSocialWorkerDetails();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
