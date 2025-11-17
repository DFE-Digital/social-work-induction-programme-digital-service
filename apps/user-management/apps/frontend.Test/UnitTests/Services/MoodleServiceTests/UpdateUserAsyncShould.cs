using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.MoodleServiceTests;

public class UpdateUserAsyncShould : MoodleServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SuccessfulRequest_ReturnsCreatedExternalUserId()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var moodleRequest = new MoodleUserRequest
        {
            Username = account.Email,
            Email = account.Email,
            FirstName = account.FirstName,
            MiddleName = account.MiddleNames,
            LastName = account.LastName
        };
        var moodleResponse = new MoodleUserResponse
        {
            Id = 1,
            Username = "Test User",
            Successful = true
        };
        MockClient.Setup(x => x.User.UpdateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest))).ReturnsAsync(moodleResponse);

        // Act
        var response = await Sut.UpdateUserAsync(account);

        // Assert
        response.Should().Be(moodleResponse.Id);
        MockClient.Verify(x => x.User.UpdateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest)), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalled_UnsuccessfulRequest_ReturnsNull()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var moodleRequest = new MoodleUserRequest
        {
            Username = account.Email,
            Email = account.Email,
            FirstName = account.FirstName,
            MiddleName = account.MiddleNames,
            LastName = account.LastName
        };
        var moodleResponse = new MoodleUserResponse { Successful = false };
        MockClient.Setup(x => x.User.UpdateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest))).ReturnsAsync(moodleResponse);

        // Act
        var actualException = await Assert.ThrowsAsync<Exception>(
            async () => await Sut.UpdateUserAsync(account)
        );

        // Assert
        actualException.Message.Should().Be($"Failed to update Moodle user with email {moodleRequest.Email}");

        MockClient.Verify(x => x.User.UpdateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest)), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
