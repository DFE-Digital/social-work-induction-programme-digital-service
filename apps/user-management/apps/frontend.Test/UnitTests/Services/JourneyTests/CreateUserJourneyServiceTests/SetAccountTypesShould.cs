using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class SetUserTypesShould : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsUserDetails()
    {
        // Arrange
        var user = UserBuilder.Build();
        var existingTypes = UserBuilder.Build().Types;
        var expected = user.Types;
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { UserTypes = existingTypes }
        );

        // Act
        Sut.SetUserTypes(expected!);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.UserTypes.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsUserDetails()
    {
        // Arrange
        var user = UserBuilder.Build();
        var expected = user.Types;

        // Act
        Sut.SetUserTypes(expected!);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.UserTypes.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }
}
