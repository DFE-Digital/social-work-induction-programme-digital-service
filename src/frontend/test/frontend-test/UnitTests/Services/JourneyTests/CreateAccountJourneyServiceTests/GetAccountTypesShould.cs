using System.Collections.Immutable;
using SocialWorkInductionProgramme.Frontend.Extensions;
using SocialWorkInductionProgramme.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetAccountTypesShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsAccountTypes()
    {
        // Arrange
        var account = AccountBuilder.Build();
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
