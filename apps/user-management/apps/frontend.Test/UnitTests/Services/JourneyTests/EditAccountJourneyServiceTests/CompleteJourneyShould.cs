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
            .WithSocialWorkEnglandNumber()
            .WithStatus(AccountStatus.Active)
            .Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);
        MockAccountService.Setup(x => x.UpdateAsync(account));

        // Act
        await Sut.CompleteJourneyAsync(account.Id);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(account.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().BeNull();

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Exactly(2));
        MockAccountService.Verify(
            x => x.UpdateAsync(MoqHelpers.ShouldBeEquivalentTo(account)),
            Times.Once
        );
        VerifyAllNoOtherCall();
    }
}
