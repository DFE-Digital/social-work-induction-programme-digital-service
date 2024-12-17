using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetAccountDetailsShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var expected = AccountDetails.FromAccount(account);
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { AccountDetails = expected }
        );

        // Act
        var response = Sut.GetAccountDetails();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AccountDetails>();
        response.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetAccountDetails();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
