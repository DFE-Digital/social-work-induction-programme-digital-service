using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public class CheckEmailShould : AccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingEmail_CallsAuthServiceAndReturnsTrue()
    {
        // Arrange
        var email = new Faker().Internet.Email();
        MockClient.Setup(x => x.Accounts.CheckEmailExistsAsync(It.Is<CheckEmailRequest>(r => r.Email == email))).ReturnsAsync(true);

        // Act
        var response = await Sut.CheckEmailExistsAsync(email);

        // Assert
        response.Should().BeTrue();
        MockClient.Verify(x => x.Accounts.CheckEmailExistsAsync(It.Is<CheckEmailRequest>(r => r.Email == email)), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalled_WithUniqueEmail_CallsAuthServiceAndReturnsFalse()
    {
        // Arrange
        var email = new Faker().Internet.Email();
        MockClient.Setup(x => x.Accounts.CheckEmailExistsAsync(It.Is<CheckEmailRequest>(r => r.Email == email))).ReturnsAsync(false);

        // Act
        var response = await Sut.CheckEmailExistsAsync(email);

        // Assert
        response.Should().BeFalse();
        MockClient.Verify(x => x.Accounts.CheckEmailExistsAsync(It.Is<CheckEmailRequest>(r => r.Email == email)), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task WhenEmailIsNullOrWhitespace_ThrowsArgumentException(string? email)
    {
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Sut.CheckEmailExistsAsync(email!));

        // Assert
        exception.Message.Should().Be("Email is required");
        MockClient.Verify(x => x.Accounts.CheckEmailExistsAsync(It.IsAny<CheckEmailRequest>()), Times.Never);
        VerifyAllNoOtherCalls();
    }
}
