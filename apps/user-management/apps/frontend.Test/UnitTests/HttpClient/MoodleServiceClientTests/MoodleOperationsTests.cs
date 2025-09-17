using System.Net;
using System.Text.Json;
using System.Web;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Options;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.MoodleServiceClientTests;

public class MoodleOperationsTests
{
    private readonly MoodleUserRequestFaker _moodleUserRequestFaker = new();
    private readonly Guid _apikey = Guid.NewGuid();
    private readonly Mock<IOptions<MoodleClientOptions>> _mockOptions = new();

    [Fact]
    public async Task CreateUserAsync_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var createUserRequest = _moodleUserRequestFaker.Generate();
        var createUserResponse = new MoodleUserResponse
        {
            Id = 1,
            Username = createUserRequest.Username,
            Successful = true
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, new List<MoodleUserResponse> { createUserResponse });

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.User.CreateUserAsync(createUserRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<MoodleUserResponse>();
        response.Should().BeEquivalentTo(createUserResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CreateUserAsync_WithUpperCaseUsername_SetsMoodleUsernameParamToLowerCase()
    {
        // Arrange
        var createUserRequest = _moodleUserRequestFaker.Generate();
        var createUserResponse = new MoodleUserResponse
        {
            Id = 1,
            Username = createUserRequest.Username,
            Successful = true
        };

        var parameters = new Dictionary<string, string>
        {
            { "wstoken", _apikey.ToString() },
            { "wsfunction", FunctionNameConstants.CreateUser },
            { "moodlewsrestformat", "json" },
            { "users[0][auth]", "oidc" },
            { "users[0][username]", createUserRequest.Email!.ToLower() },
            { "users[0][firstname]", createUserRequest.FirstName! },
            { "users[0][lastname]", createUserRequest.LastName! },
            { "users[0][email]", createUserRequest.Email! }
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, new List<MoodleUserResponse> { createUserResponse }, parameters);

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.User.CreateUserAsync(createUserRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<MoodleUserResponse>();
        response.Should().BeEquivalentTo(createUserResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CreateUserAsync_WhenErrorResponseReturned_ReturnsErrorResponse()
    {
        // Arrange
        var createUserRequest = _moodleUserRequestFaker.Generate();
        var createUserResponse = new MoodleUserResponse { Successful = false };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.BadRequest, new List<MoodleUserResponse> { createUserResponse });

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.User.CreateUserAsync(createUserRequest);

        // Assert
        response.Should().BeEquivalentTo(createUserResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Theory]
    [InlineData("", "smith", "test@test.com", "user1")]
    [InlineData("John", "", "test@test.com", "user1")]
    [InlineData("John", "smith", "", "user1")]
    [InlineData("John", "smith", "test@test.com", "")]
    public async Task CreateUserAsync_WhenNullValuesPassedIn_ReturnsErrorResponse(
        string firstName,
        string lastName,
        string email,
        string username
    )
    {
        // Arrange
        var createUserRequest = new MoodleUserRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username
        };
        var createUserResponse = new MoodleUserResponse { Successful = false };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.BadRequest, new List<MoodleUserResponse> { createUserResponse });

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.User.CreateUserAsync(createUserRequest);

        // Assert
        response.Should().BeEquivalentTo(createUserResponse);

        mockHttp.GetMatchCount(request).Should().Be(0);
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
        ) GenerateMockClient(
            HttpStatusCode statusCode,
            IList<MoodleUserResponse> response,
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

    [Fact]
    public async Task UpdateUserAsync_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var updateUserRequest = _moodleUserRequestFaker.Generate();
        var updateUserResponse = new MoodleUserResponse
        {
            Id = updateUserRequest.Id,
            Username = updateUserRequest.Email?.ToLower(),
            Successful = true
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, new List<MoodleUserResponse> { updateUserResponse });

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.User.UpdateUserAsync(updateUserRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<MoodleUserResponse>();
        response.Should().BeEquivalentTo(updateUserResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
