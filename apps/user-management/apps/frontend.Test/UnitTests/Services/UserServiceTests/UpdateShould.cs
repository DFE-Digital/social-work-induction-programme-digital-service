using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.UserServiceTests;

public class UpdateShould : UserServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CallsAuthServive_ReturnsPerson()
    {
        // Arrange
        var person = PersonFaker.Generate();
        var userToUpdate = Mapper.MapToBo(person);

        // Act
        var response = await Sut.UpdateAsync(userToUpdate);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<User>();
        response
            .Should()
            .Match<User>(user =>
                user.Email == person.EmailAddress
                && user.FirstName == person.FirstName
                && user.LastName == person.LastName
                && user.SocialWorkEnglandNumber == person.SocialWorkEnglandNumber
                && user.Types == person.Roles
            );

        MockClient.Verify(
            x =>
                x.Users.UpdateAsync(
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
        var userToUpdate = new User
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };

        // Act
        var actualException = await Assert.ThrowsAsync<ArgumentException>(
            async () => await Sut.UpdateAsync(userToUpdate)
        );

        // Assert
        actualException.Should().BeOfType<ArgumentException>();
        actualException
            .Message.Should()
            .Be("Person ID, First name, last name, and email are required");
        VerifyAllNoOtherCalls();
    }
}
