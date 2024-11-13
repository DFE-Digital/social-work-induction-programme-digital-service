using System.Net;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public class AccountsOperationsTests
{
    private readonly Mock<IOptions<AuthClientOptions>> _mockOptions;

    public AccountsOperationsTests()
    {
        _mockOptions = new();
    }

    [Fact]
    public async Task GetAll_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var route = "/api/Accounts";
        var persons = new List<Person>
        {
            new()
            {
                PersonId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Trn = "1234",
                FirstName = "Test",
                LastName = "McTester",
            }
        };

        var paginationRequest = new PaginationRequest(0, 10);
        var paginationResponse = new PaginationResult<Person>
        {
            Records = persons,
            MetaData = new PaginationMetaData
            {
                Page = 1,
                PageSize = 5,
                PageCount = 2,
                TotalCount = 10,
                Links = new Dictionary<string, MetaDataLink>()
            }
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, paginationResponse, route);

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Accounts.GetAllAsync(paginationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(paginationResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetAll_WhenErrorResponseReturned_ReturnsNull()
    {
        // Arrange
        var route = $"/api/Accounts";

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.BadRequest, null, route);

        var sut = BuildSut(mockHttp);

        var paginationRequest = new PaginationRequest(0, 10);

        // Act
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.Accounts.GetAllAsync(paginationRequest)
        );

        // Assert
        actualException.Should().BeOfType<InvalidOperationException>();

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var route = $"/api/Accounts/{personId}";
        var person = new Person
        {
            PersonId = personId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            Trn = "1234",
            FirstName = "Test",
            LastName = "McTester",
        };

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, person, route);

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Accounts.GetByIdAsync(personId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Person>();
        response.Should().BeEquivalentTo(person);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_WhenErrorResponseReturned_ReturnsNull()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var route = $"/api/Accounts/{personId}";

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.BadRequest, null, route);

        var sut = BuildSut(mockHttp);

        // Act
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.Accounts.GetByIdAsync(personId)
        );

        // Assert
        actualException.Should().BeOfType<InvalidOperationException>();

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    private AuthServiceClient BuildSut(MockHttpMessageHandler mockHttpMessageHandler)
    {
        var client = mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        _mockOptions
            .Setup(x => x.Value)
            .Returns(
                new AuthClientOptions
                {
                    BaseUrl = "http://localhost",
                    ClientCredentials = new ClientCredentials
                    {
                        ClientId = string.Empty,
                        ClientSecret = string.Empty,
                        AccessTokenUrl = string.Empty
                    }
                }
            );

        var sut = new AuthServiceClient(client);

        return sut;
    }

    private static (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
    ) GenerateMockClient(HttpStatusCode statusCode, object? response, string route)
    {
        using var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(HttpMethod.Get, route)
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }
}
