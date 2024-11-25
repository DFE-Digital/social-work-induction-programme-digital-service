using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class SetAccountDetailsShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_SetsAccountDetails()
    {
        // Arrange
        var originalAccount = AccountFaker.Generate();

        var updatedAccount = AccountFaker.Generate();
        var updatedAccountDetails = AccountDetails.FromAccount(updatedAccount);
        var expected = new EditAccountJourneyModel(updatedAccount).AccountDetails;

        MockAccountRepository.Setup(x => x.GetById(originalAccount.Id)).Returns(originalAccount);

        // Act
        Sut.SetAccountDetails(originalAccount.Id, updatedAccountDetails);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(originalAccount.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().NotBeNull();
        editAccountJourneyModel!.AccountDetails.Should().BeEquivalentTo(expected);

        MockAccountRepository.Verify(x => x.GetById(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
