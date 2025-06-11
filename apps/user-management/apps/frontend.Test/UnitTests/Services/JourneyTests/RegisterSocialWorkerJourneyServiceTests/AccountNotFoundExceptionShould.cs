using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests;

public class AccountNotFoundExceptionShould : RegisterSocialWorkerJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ThrowsException()
    {
        // Arrange
        var account = AccountBuilder.Build();

        var expectedException = new KeyNotFoundException("Account not found with ID " + account.Id);

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync((Account?)null);

        // Act
        var actualException = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => Sut.SetDateOfBirthAsync(account.Id, DateTime.UtcNow)
        );

        // Assert
        actualException.Message.Should().Be(expectedException.Message);

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
