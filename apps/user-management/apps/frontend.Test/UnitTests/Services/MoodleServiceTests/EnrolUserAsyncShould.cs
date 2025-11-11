using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.MoodleServiceTests;

public class EnrolUserAsyncShould : MoodleServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SuccessfulRequest_ReturnsCreatedExternalOrgId()
    {
        // Arrange
        var externalUserId = 100;
        var externalOrgId = 200;
        var roleId = MoodleRoles.Manager;

        var enrolUserRequest = new EnrolUserRequest
        {
            UserId = externalUserId,
            CourseId = externalOrgId,
            RoleId = roleId
        };

        var enrolUserResponse = new EnrolUserResponse
        {
            Successful = true
        };

        MockClient.Setup(x => x.Course.EnrolUserAsync(MoqHelpers.ShouldBeEquivalentTo(enrolUserRequest))).ReturnsAsync(enrolUserResponse);

        // Act
        var response = await Sut.EnrolUserAsync(externalUserId, externalOrgId, roleId);

        // Assert
        response.Should().BeTrue();
        MockClient.Verify(x => x.Course.EnrolUserAsync(MoqHelpers.ShouldBeEquivalentTo(enrolUserRequest)), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalled_UnsuccessfulRequest_ReturnsNull()
    {
        // Arrange
        var externalUserId = 100;
        var externalOrgId = 200;
        var roleId = MoodleRoles.Manager;

        var enrolUserRequest = new EnrolUserRequest
        {
            UserId = externalUserId,
            CourseId = externalOrgId,
            RoleId = roleId
        };

        var enrolUserResponse = new EnrolUserResponse
        {
            Successful = false
        };

        MockClient.Setup(x => x.Course.EnrolUserAsync(MoqHelpers.ShouldBeEquivalentTo(enrolUserRequest))).ReturnsAsync(enrolUserResponse);

        // Act
        var actualException = await Assert.ThrowsAsync<Exception>(
            async () => await Sut.EnrolUserAsync(externalUserId, externalOrgId, roleId)
        );

        // Assert
        actualException.Message.Should().Be($"Failed to enrol user (ID {externalUserId}) onto Course (ID {externalOrgId}");

        MockClient.Verify(x => x.Course.EnrolUserAsync(MoqHelpers.ShouldBeEquivalentTo(enrolUserRequest)), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
