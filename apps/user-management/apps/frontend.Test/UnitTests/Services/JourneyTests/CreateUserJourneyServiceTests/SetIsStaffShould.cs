using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;

public class SetIsStaffShould : CreateUserJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsUserDetails()
    {
        // Arrange
        var account = UserBuilder.Build();
        var existingIsStaff = UserBuilder.Build().IsStaff;
        var expected = !account.IsStaff;
        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel { IsStaff = existingIsStaff }
        );

        // Act
        Sut.SetIsStaff(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.IsStaff.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsUserDetails()
    {
        // Arrange
        var account = UserBuilder.Build();
        var expected = !account.IsStaff;

        // Act
        Sut.SetIsStaff(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );

        createUserJourneyModel.Should().NotBeNull();
        createUserJourneyModel!.IsStaff.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
