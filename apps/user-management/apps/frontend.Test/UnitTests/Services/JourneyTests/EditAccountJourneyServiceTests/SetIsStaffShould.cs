using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class SetIsStaffShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_SetsIsStaff()
    {
        // Arrange
        var originalAccount = AccountFaker.Generate();

        MockAccountRepository.Setup(x => x.GetById(originalAccount.Id)).Returns(originalAccount);

        // Act
        Sut.SetIsStaff(originalAccount.Id, !originalAccount.IsStaff);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(originalAccount.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().NotBeNull();
        editAccountJourneyModel!.IsStaff.Should().Be(!originalAccount.IsStaff);

        MockAccountRepository.Verify(x => x.GetById(originalAccount.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
