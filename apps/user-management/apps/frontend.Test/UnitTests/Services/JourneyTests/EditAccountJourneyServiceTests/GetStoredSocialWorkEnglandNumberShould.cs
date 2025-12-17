using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class GetStoredSocialWorkEnglandNumberShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnSocialWorkEnglandNumber()
    {
        // Arrange
        var account = AccountBuilder.Build();

        var expected = account.SocialWorkEnglandNumber;

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var response = await Sut.GetStoredSocialWorkEnglandNumberAsync(account.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<string>();
        response.Should().BeEquivalentTo(expected);

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        VerifyAllNoOtherCall();
    }
}
