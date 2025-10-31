using System.Net;
using System.Text.Json;
using System.Web;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Options;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.MoodleServiceClientTests;

public class CourseOperationsTests
{
    private readonly MoodleEnrolUserOntoCourseRequestFaker _moodleCourseRequestFaker = new();
    private readonly Guid _apikey = Guid.NewGuid();
    private readonly Mock<IOptions<MoodleClientOptions>> _mockOptions = new();

    [Fact]
    public async Task CreateAsync_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var createCourseRequest = new CreateCourseRequest
        {
            FullName = "Full Course Name",
            ShortName = "Short Name",
            CategoryId = 1
        };

        var createCourseResponse = new CreateCourseResponse
        {
            Id = 5,
            ShortName = createCourseRequest.ShortName,
            Successful = true
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, new List<CreateCourseResponse> { createCourseResponse });

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Course.CreateAsync(createCourseRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<CreateCourseResponse>();
        response.Should().BeEquivalentTo(createCourseResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CreateAsync_UnsuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var createCourseRequest = new CreateCourseRequest
        {
            FullName = "Full Course Name",
            ShortName = "Short Name",
            CategoryId = 1
        };

        var createCourseResponse = new CreateCourseResponse
        {
            Successful = false
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.BadRequest, new List<CreateCourseResponse> { createCourseResponse });

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Course.CreateAsync(createCourseRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<CreateCourseResponse>();
        response.Should().BeEquivalentTo(createCourseResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task EnrolUserAsync_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var enrolUserRequest = _moodleCourseRequestFaker.Generate();
        var enrolUserResponse = new EnrolUserResponse();

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, enrolUserResponse);

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Course.EnrolUserAsync(enrolUserRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<EnrolUserResponse>();
        response.Should().BeEquivalentTo(enrolUserResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    private MoodleServiceClient BuildSut(MockHttpMessageHandler mockHttpMessageHandler)
    {
        var client = mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        _mockOptions
            .Setup(x => x.Value)
            .Returns(
                new MoodleClientOptions
                {
                    BaseUrl = "http://localhost",
                    ApiToken = _apikey.ToString()
                }
            );

        var sut = new MoodleServiceClient(client, _mockOptions.Object);

        return sut;
    }

    private static (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
        ) GenerateMockClient<TRequest>(
            HttpStatusCode statusCode,
            TRequest response,
            IDictionary<string, string>? expectedFormParams = null)
    {
        using var mockHttp = new MockHttpMessageHandler();

        var request = mockHttp
            .When(HttpMethod.Post, string.Empty)
            .With(req =>
            {
                if (expectedFormParams is null)
                    return true;

                var body = req.Content!.ReadAsStringAsync().Result;
                var parsed = HttpUtility.ParseQueryString(body);

                // all expected params must match
                return expectedFormParams.All(kvp => parsed[kvp.Key] == kvp.Value);
            })
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }
}
