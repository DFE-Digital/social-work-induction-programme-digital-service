using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class GetAccountDetailsShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnAccountDetails()
    {
        // Arrange
        var account = AccountBuilder.Build();

        var expected = new EditAccountJourneyModel(account);

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var response = await Sut.GetAccountDetailsAsync(account.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AccountDetails>();
        response.Should().BeEquivalentTo(expected.AccountDetails);

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
