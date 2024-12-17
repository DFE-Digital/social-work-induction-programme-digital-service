using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class ResetCreateAccountJourneyModelShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ResetsUserJourney()
    {
        // Arrange
        var account = AccountFaker.Generate();
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel
            {
                AccountDetails = AccountDetails.FromAccount(account),
                AccountTypes = account.Types,
                IsStaff = account.IsStaff
            }
        );

        // Act
        Sut.ResetCreateAccountJourneyModel();

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().BeNull();

        VerifyAllNoOtherCall();
    }
}
