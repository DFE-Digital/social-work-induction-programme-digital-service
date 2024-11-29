using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public class UpdateShould : AccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ThrowsNotImplementedException()
    {
        // Arrange
        var account = AccountFaker.Generate();

        // Act
        var actualException = await Assert.ThrowsAsync<NotImplementedException>(
            async () => await Sut.UpdateAsync(account)
        );

        // Assert
        actualException.Should().BeOfType<NotImplementedException>();

        VerifyAllNoOtherCalls();
    }
}
