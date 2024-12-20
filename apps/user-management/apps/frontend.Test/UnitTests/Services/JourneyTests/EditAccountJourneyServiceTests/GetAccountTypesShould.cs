using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class GetAccountTypesShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnAccountTypes()
    {
        // Arrange
        var account = AccountBuilder.Build();

        var expected = new EditAccountJourneyModel(account);

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var response = await Sut.GetAccountTypesAsync(account.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ImmutableList<AccountType>?>();
        response.Should().BeEquivalentTo(expected.AccountTypes);

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
