using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class SetAccountDetailsShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsAccountDetails()
    {
        // Arrange
        var originalAccount = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();

        var updatedAccount = AccountBuilder.Build();
        var updatedAccountDetails = AccountDetails.FromAccount(updatedAccount);
        var expected = new EditAccountJourneyModel(updatedAccount).AccountDetails;

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.SetAccountDetailsAsync(originalAccount.Id, updatedAccountDetails);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(originalAccount.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().NotBeNull();
        editAccountJourneyModel!.AccountDetails.Should().BeEquivalentTo(expected);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
