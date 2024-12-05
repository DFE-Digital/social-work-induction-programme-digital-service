using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.AccountServiceTests;

public class UpdateShould : AccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CallsAuthServive_ReturnsPerson()
    {
        // Arrange
        var person = PersonFaker.Generate();
        var accountToUpdate = Mapper.MapToBo(person);

        // Act
        var response = await Sut.UpdateAsync(accountToUpdate);

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
                x.Accounts.UpdateAsync(
                    It.Is<UpdatePersonRequest>(request =>
                        request.EmailAddress == person.EmailAddress
                        && request.FirstName == person.FirstName
                        && request.LastName == person.LastName
                    )
                ),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledWithNullData_ThrowsArgumentException()
    {
        // Arrange
        var accountToUpdate = new Account
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };

        // Act
        var actualException = await Assert.ThrowsAsync<ArgumentException>(
            async () => await Sut.UpdateAsync(accountToUpdate)
        );

        // Assert
        actualException.Should().BeOfType<ArgumentException>();
        actualException
            .Message.Should()
            .Be("Person ID, First name, last name, and email are required");
        VerifyAllNoOtherCalls();
    }
}
