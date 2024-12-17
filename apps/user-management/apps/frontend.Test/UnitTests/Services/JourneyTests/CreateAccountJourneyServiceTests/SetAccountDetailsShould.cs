using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetAccountDetailsShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var existingDetails = AccountDetails.FromAccount(AccountFaker.Generate());
        var expected = AccountDetails.FromAccount(account);
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { AccountDetails = existingDetails }
        );

        // Act
        Sut.SetAccountDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.AccountDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var expected = AccountDetails.FromAccount(account);

        // Act
        Sut.SetAccountDetails(expected);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.AccountDetails.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }
}
