using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public class CreateShould : AccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CallsAuthServiceAndReturnsNewAccount()
    {
        // Arrange
        var person = PersonFaker.Generate();
        var accountToCreate = Mapper.MapToBo(person);

        // Act
        var response = await Sut.CreateAsync(accountToCreate);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Account>();
        response
            .Should()
            .Match<Account>(account =>
                account.Email == person.EmailAddress
                && account.FirstName == person.FirstName
                && account.LastName == person.LastName
                && account.SocialWorkEnglandNumber == person.SocialWorkEnglandNumber
                && account.Types == person.Roles
            );

        MockClient.Verify(
            x =>
                x.Accounts.CreateAsync(
                    It.Is<CreatePersonRequest>(request =>
                        request.EmailAddress == person.EmailAddress
                        && request.FirstName == person.FirstName
                        && request.LastName == person.LastName
                    )
                ),
            Times.Once
        );
        MockClient.Verify(
            x =>
                x.HttpContextService.GetOrganisationId(),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledWithNullData_ThrowsArgumentException()
    {
        // Arrange
        var accountToCreate = new Account
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };

        // Act
        var actualException = await Assert.ThrowsAsync<ArgumentException>(
            async () => await Sut.CreateAsync(accountToCreate)
        );

        // Assert
        actualException.Should().BeOfType<ArgumentException>();
        actualException.Message.Should().Be("First name, last name, and email are required");
        VerifyAllNoOtherCalls();
    }
}
