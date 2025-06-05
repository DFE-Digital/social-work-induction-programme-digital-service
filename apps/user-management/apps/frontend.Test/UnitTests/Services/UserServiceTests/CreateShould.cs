using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.UserServiceTests;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.UserServiceTests;

public class CreateShould : UserServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CallsAuthServiceAndReturnsNewUser()
    {
        // Arrange
        var person = PersonFaker.Generate();
        var userToCreate = Mapper.MapToBo(person);

        // Act
        var response = await Sut.CreateAsync(userToCreate);

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
                x.Users.CreateAsync(
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
        var userToCreate = new User
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };

        // Act
        var actualException = await Assert.ThrowsAsync<ArgumentException>(
            async () => await Sut.CreateAsync(userToCreate)
        );

        // Assert
        actualException.Should().BeOfType<ArgumentException>();
        actualException.Message.Should().Be("First name, last name, and email are required");
        VerifyAllNoOtherCalls();
    }
}
