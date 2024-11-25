using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class ResetCreateAccountJourneyModelShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ResetsUserJourney()
    {
        // Arrange
        var account = AccountFaker.Generate();

        MockAccountRepository.Setup(x => x.GetById(account.Id)).Returns(account);

        Sut.SetIsStaff(account.Id, account.IsStaff);

        // Act
        Sut.ResetCreateAccountJourneyModel(account.Id);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(account.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().BeNull();

        MockAccountRepository.Verify(x => x.GetById(account.Id), Times.Exactly(2));
        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenAccountNotFound_ThrowExpectedException()
    {
        // Arrange
        var account = AccountFaker.Generate();

        var expectedException = new KeyNotFoundException("Account not found with ID " + account.Id);

        MockAccountRepository.Setup(x => x.GetById(account.Id)).Returns((Account?)null);

        // Act
        var actualException = Assert.Throws<KeyNotFoundException>(
            () => Sut.ResetCreateAccountJourneyModel(account.Id)
        );

        // Assert
        actualException.Message.Should().Be(expectedException.Message);

        MockAccountRepository.Verify(x => x.GetById(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
