using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class SetIsStaffShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SetsIsStaff()
    {
        // Arrange
        var originalAccount = AccountBuilder.Build();

        MockAccountService
            .Setup(x => x.GetByIdAsync(originalAccount.Id))
            .ReturnsAsync(originalAccount);

        // Act
        await Sut.SetIsStaffAsync(originalAccount.Id, !originalAccount.IsStaff);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(originalAccount.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().NotBeNull();
        editAccountJourneyModel!.IsStaff.Should().Be(!originalAccount.IsStaff);

        MockAccountService.Verify(x => x.GetByIdAsync(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
