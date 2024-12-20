using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsStaffShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsAccountTypes()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var expected = account.IsStaff;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { IsStaff = expected }
        );

        // Act
        var response = Sut.GetIsStaff();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetIsStaff();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
