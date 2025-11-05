using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.MoodleServiceTests;

public class CreateCourseAsyncShould : MoodleServiceTestBase
{
    [Fact]
    public async Task WhenCalled_SuccessfulRequest_ReturnsCreatedExternalOrgId()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var moodleRequest = new CreateCourseRequest
        {
            FullName = organisation.OrganisationName,
            ShortName = organisation.OrganisationName,
            CategoryId = 1
        };
        var moodleResponse = new CreateCourseResponse
        {
            Id = 1,
            ShortName = organisation.OrganisationName,
            Successful = true
        };
        MockClient.Setup(x => x.Course.CreateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest))).ReturnsAsync(moodleResponse);

        // Act
        var response = await Sut.CreateCourseAsync(organisation);

        // Assert
        response.Should().Be(moodleResponse.Id);
        MockClient.Verify(x => x.Course.CreateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest)), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalled_UnsuccessfulRequest_ReturnsNull()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var moodleRequest = new CreateCourseRequest
        {
            FullName = organisation.OrganisationName,
            ShortName = organisation.OrganisationName,
            CategoryId = 1
        };
        var moodleResponse = new CreateCourseResponse { Successful = false };
        MockClient.Setup(x => x.Course.CreateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest))).ReturnsAsync(moodleResponse);

        // Act
        var actualException = await Assert.ThrowsAsync<Exception>(
            async () => await Sut.CreateCourseAsync(organisation)
        );

        // Assert
        actualException.Message.Should().Be($"Failed to create Moodle course with name {organisation.OrganisationName}");

        MockClient.Verify(x => x.Course.CreateAsync(MoqHelpers.ShouldBeEquivalentTo(moodleRequest)), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
