using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class SetAccountStatusShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsAccountStatus()
    {
        // Arrange
        var originalAccount = AccountFaker.Generate();

        var updatedAccount = AccountFaker.Generate();
        var editedAccountStatus = new EditAccountJourneyModel(updatedAccount).AccountStatus!;

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.SetAccountStatusAsync(originalAccount.Id, editedAccountStatus.Value);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(originalAccount.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().NotBeNull();
        editAccountJourneyModel!.AccountStatus.Should().Be(editedAccountStatus.Value);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
