using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class GetUserTypesShould : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsUserTypes()
    {
        // Arrange
        var user = UserBuilder.Build();
        var expected = user.Types;
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { UserTypes = expected }
        );

        // Act
        var response = Sut.GetUserTypes();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ImmutableList<UserType>>();
        response.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetUserTypes();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
