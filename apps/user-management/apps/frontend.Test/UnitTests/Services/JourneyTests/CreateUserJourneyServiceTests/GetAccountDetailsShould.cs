using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class GetUserDetailsShould : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsUserDetails()
    {
        // Arrange
        var user = UserBuilder.Build();
        var expected = UserDetails.FromUser(user);
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { UserDetails = expected }
        );

        // Act
        var response = Sut.GetUserDetails();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<UserDetails>();
        response.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetUserDetails();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
