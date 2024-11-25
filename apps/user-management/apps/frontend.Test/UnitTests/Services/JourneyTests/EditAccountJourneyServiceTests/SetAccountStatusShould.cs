using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class SetAccountStatusShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_SetsAccountStatus()
    {
        // Arrange
        var originalAccount = AccountFaker.Generate();

        var updatedAccount = AccountFaker.Generate();
        var editedAccountStatus = new EditAccountJourneyModel(updatedAccount).AccountStatus!;

        MockAccountRepository.Setup(x => x.GetById(originalAccount.Id)).Returns(originalAccount);

        // Act
        Sut.SetAccountStatus(originalAccount.Id, editedAccountStatus.Value);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(originalAccount.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().NotBeNull();
        editAccountJourneyModel!.AccountStatus.Should().Be(editedAccountStatus.Value);

        MockAccountRepository.Verify(x => x.GetById(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
