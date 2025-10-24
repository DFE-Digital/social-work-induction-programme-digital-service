using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsFundedShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsIsFunded()
    {
        // Arrange
        var expected = true;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel
            {
                AccountTypes = [AccountType.EarlyCareerSocialWorker],
                IsRegisteredWithSocialWorkEngland = true,
                IsStatutoryWorker = true,
                IsAgencyWorker = false,
                IsRecentlyQualified = true,
                IsEnrolledInAsye = false
            }
        );

        // Act
        var response = Sut.GetIsFunded();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithDefaultSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetIsFunded();

        // Assert
        response.Should().BeFalse();

        VerifyAllNoOtherCall();
    }
}
