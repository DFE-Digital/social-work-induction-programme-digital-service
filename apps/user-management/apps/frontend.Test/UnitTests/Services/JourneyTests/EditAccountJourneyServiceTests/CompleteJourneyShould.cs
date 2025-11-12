using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class CompleteJourneyShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CompletesJourney()
    {
        // Arrange
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithStatus(AccountStatus.Active)
            .Build();
        var externalUserId = 100;

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);
        MockFeatureFlags.SetupGet(x => x.Value).Returns(new FeatureFlags { EnableMoodleIntegration = true });
        MockMoodleService.Setup(x => x.UpdateUserAsync(It.Is<Account>(acc => acc.Email == account.Email))).ReturnsAsync(externalUserId);

        // Act
        await Sut.CompleteJourneyAsync(account.Id);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(account.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().BeNull();

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Exactly(2));
        MockMoodleService.Verify(x => x.UpdateUserAsync(It.Is<Account>(acc => acc.Email == account.Email)), Times.Once);
        MockAccountService.Verify(x => x.UpdateAsync(It.Is<Account>(acc => acc.Email == account.Email && acc.ExternalUserId == externalUserId)), Times.Once);
        VerifyAllNoOtherCall();
    }
}
