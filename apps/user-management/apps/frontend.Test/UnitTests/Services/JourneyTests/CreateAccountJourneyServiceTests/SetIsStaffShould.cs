using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetIsStaffShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsAccountDetails()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var existingIsStaff = AccountBuilder.Build().IsStaff;
        var expected = !account.IsStaff;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsStaff = existingIsStaff }
        );

        // Act
        Sut.SetIsStaff(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsStaff.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsAccountDetails()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var expected = !account.IsStaff;

        // Act
        Sut.SetIsStaff(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.IsStaff.Should().Be(expected);

        VerifyAllNoOtherCall();
    }
}
