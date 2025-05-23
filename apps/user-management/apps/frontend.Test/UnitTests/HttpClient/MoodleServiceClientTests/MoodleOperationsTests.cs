﻿using System.Net;
using System.Text.Json;
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
    private readonly CreateMoodleUserRequestFaker _createMoodleUserRequestFaker = new();
    private readonly Guid _apikey = Guid.NewGuid();
    private readonly Mock<IOptions<MoodleClientOptions>> _mockOptions = new();

    [Fact]
    public async Task CreateUserAsync_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var createUserRequest = _createMoodleUserRequestFaker.Generate();
        var createUserResponse = new CreateMoodleUserResponse
        {
            Id = 1,
            Username = createUserRequest.Username,
            Successful = true
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, new List<CreateMoodleUserResponse> { createUserResponse });

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.User.CreateUserAsync(createUserRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<CreateMoodleUserResponse>();
        response.Should().BeEquivalentTo(createUserResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CreateUserAsync_WhenErrorResponseReturned_ReturnsErrorResponse()
    {
        // Arrange
        var createUserRequest = _createMoodleUserRequestFaker.Generate();
        var createUserResponse = new CreateMoodleUserResponse { Successful = false };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.BadRequest, new List<CreateMoodleUserResponse> { createUserResponse });

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
        var createUserRequest = new CreateMoodleUserRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username
        };
        var createUserResponse = new CreateMoodleUserResponse { Successful = false };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.BadRequest, new List<CreateMoodleUserResponse> { createUserResponse });

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
    ) GenerateMockClient(HttpStatusCode statusCode, IList<CreateMoodleUserResponse> response)
    {
        using var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(HttpMethod.Post, string.Empty)
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }
}
