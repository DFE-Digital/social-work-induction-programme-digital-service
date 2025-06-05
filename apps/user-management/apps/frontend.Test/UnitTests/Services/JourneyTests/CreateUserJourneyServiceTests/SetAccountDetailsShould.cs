using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class SetUserDetailsShould : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsUserDetails()
    {
        // Arrange
        var user = UserBuilder.Build();
        var existingDetails = UserDetails.FromUser(UserBuilder.Build());
        var expected = UserDetails.FromUser(user);
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { UserDetails = existingDetails }
        );

        // Act
        Sut.SetUserDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.UserDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsUserDetails()
    {
        // Arrange
        var account = UserBuilder.Build();
        var expected = UserDetails.FromUser(account);

        // Act
        Sut.SetUserDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.UserDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }
}
