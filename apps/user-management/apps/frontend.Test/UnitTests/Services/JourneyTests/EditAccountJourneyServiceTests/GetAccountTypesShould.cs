using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class GetAccountTypesShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ReturnAccountTypes()
    {
        // Arrange
        var account = AccountFaker.Generate();

        var expected = new EditAccountJourneyModel(account);

        MockAccountRepository.Setup(x => x.GetById(account.Id)).Returns(account);

        // Act
        var response = Sut.GetAccountTypes(account.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ImmutableList<AccountType>?>();
        response.Should().BeEquivalentTo(expected.AccountTypes);

        MockAccountRepository.Verify(x => x.GetById(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
