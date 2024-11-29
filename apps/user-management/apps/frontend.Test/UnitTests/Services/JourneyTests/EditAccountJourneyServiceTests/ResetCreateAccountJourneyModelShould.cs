using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class ResetCreateAccountJourneyModelShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ResetsUserJourney()
    {
        // Arrange
        var account = AccountFaker.Generate();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        await Sut.SetIsStaffAsync(account.Id, account.IsStaff);

        // Act
        await Sut.ResetCreateAccountJourneyModelAsync(account.Id);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(account.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().BeNull();

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Exactly(2));
        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenAccountNotFound_ThrowExpectedException()
    {
        // Arrange
        var account = AccountFaker.Generate();

        var expectedException = new KeyNotFoundException("Account not found with ID " + account.Id);

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync((Account?)null);

        // Act
        var actualException = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => Sut.ResetCreateAccountJourneyModelAsync(account.Id)
        );

        // Assert
        actualException.Message.Should().Be(expectedException.Message);

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
