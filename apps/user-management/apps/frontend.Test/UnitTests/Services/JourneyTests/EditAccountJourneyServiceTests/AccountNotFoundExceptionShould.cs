using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class AccountNotFoundExceptionShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ThrowsException()
    {
        // Arrange
        var account = AccountFaker.Generate();

        var expectedException = new KeyNotFoundException("Account not found with ID " + account.Id);

        MockAccountRepository.Setup(x => x.GetById(account.Id)).Returns((Account?)null);

        // Act
        var actualException = Assert.Throws<KeyNotFoundException>(() => Sut.GetIsStaff(account.Id));

        // Assert
        actualException.Message.Should().Be(expectedException.Message);

        MockAccountRepository.Verify(x => x.GetById(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
