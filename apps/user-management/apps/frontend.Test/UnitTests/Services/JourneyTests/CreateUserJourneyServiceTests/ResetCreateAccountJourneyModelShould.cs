using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class ResetCreateUserJourneyModelShould : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ResetsUserJourney()
    {
        // Arrange
        var user = UserBuilder.Build();
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel
            {
                UserDetails = UserDetails.FromUser(user),
                UserTypes = user.Types,
                IsStaff = user.IsStaff
            }
        );

        // Act
        Sut.ResetCreateUserJourneyModel();

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
