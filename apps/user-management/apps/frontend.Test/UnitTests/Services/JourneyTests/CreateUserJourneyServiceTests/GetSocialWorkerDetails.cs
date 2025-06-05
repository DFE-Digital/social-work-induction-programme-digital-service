using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class GetSocialWorkerDetails : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsUserDetails()
    {
        // Arrange
        var expected = new SocialWorkerFaker().Generate();
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { SocialWorkerDetails = expected }
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
