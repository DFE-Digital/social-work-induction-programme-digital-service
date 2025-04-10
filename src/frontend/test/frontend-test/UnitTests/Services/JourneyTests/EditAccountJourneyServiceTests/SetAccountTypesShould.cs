using SocialWorkInductionProgramme.Frontend.Extensions;
using SocialWorkInductionProgramme.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class SetAccountTypesShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsAccountTypes()
    {
        // Arrange
        var originalAccount = AccountBuilder.Build();

        var updatedAccount = AccountBuilder.Build();
        var editedAccountTypes = new EditAccountJourneyModel(updatedAccount).AccountTypes!;

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.SetAccountTypesAsync(originalAccount.Id, editedAccountTypes);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(originalAccount.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().NotBeNull();
        editAccountJourneyModel!.AccountTypes.Should().BeEquivalentTo(editedAccountTypes);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
