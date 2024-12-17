using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetAccountTypesShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsAccountTypes()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var expected = account.Types;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { AccountTypes = expected }
        );

        // Act
        var response = Sut.GetAccountTypes();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ImmutableList<AccountType>>();
        response.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetAccountTypes();

        // Assert
        response.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
