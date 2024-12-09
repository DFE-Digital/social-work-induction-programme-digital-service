using System.Net;
using System.Text.Json;
using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Options;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;
using Person = Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Person;

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
                SocialWorkEnglandNumber = "1234",
                FirstName = "Test",
                LastName = "McTester"
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

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            paginationResponse,
            route
        );

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

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Get,
            null,
            route
        );

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
            SocialWorkEnglandNumber = "1234",
            FirstName = "Test",
            LastName = "McTester",
        };

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            person,
            route
        );

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

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Accounts.GetByIdAsync(personId);

        // Assert
        response.Should().BeNull();

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Create_SuccessfulRequest_ReturnsPerson()
    {
        // Arrange
        var person = new Person
        {
            PersonId = Guid.NewGuid(),
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            SocialWorkEnglandNumber = "1234",
            FirstName = "Test",
            LastName = "McTester",
            EmailAddress = "test@test.com",
            Status = new Faker().PickRandom<AccountStatus>()
        };

        var createRequest = new CreatePersonRequest
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            SocialWorkEnglandNumber = person.SocialWorkEnglandNumber,
            EmailAddress = person.EmailAddress,
            Status = person.Status
        };

        const string route = "/api/Accounts/Create";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Post,
            person,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Accounts.CreateAsync(createRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(person);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Create_WhenErrorResponseReturned_ReturnsNull()
    {
        // Arrange
        var createRequest = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "test@test.com"
        };
        var route = "/api/Accounts/Create";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Post,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.Accounts.CreateAsync(createRequest)
        );

        // Assert
        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Update_SuccessfulRequest_ReturnsPerson()
    {
        // Arrange
        var person = new Person
        {
            PersonId = Guid.NewGuid(),
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            SocialWorkEnglandNumber = "1234",
            FirstName = "Test",
            LastName = "McTester",
            EmailAddress = "test@test.com",
            Status = new Faker().PickRandom<AccountStatus>()
        };

        var updateRequest = new UpdatePersonRequest
        {
            PersonId = person.PersonId,
            FirstName = person.FirstName,
            LastName = person.LastName,
            SocialWorkEnglandNumber = person.SocialWorkEnglandNumber,
            EmailAddress = person.EmailAddress,
            Status = person.Status
        };

        var route = "/api/Accounts";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Put,
            person,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Accounts.UpdateAsync(updateRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(person);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Update_WhenErrorResponseReturned_ReturnsNull()
    {
        // Arrange
        var updateRequest = new UpdatePersonRequest
        {
            PersonId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "test@test.com"
        };
        var route = "/api/Accounts";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Put,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.Accounts.UpdateAsync(updateRequest)
        );

        // Assert
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
    ) GenerateMockClient(
        HttpStatusCode statusCode,
        HttpMethod httpMethod,
        object? response,
        string route
    )
    {
        using var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(httpMethod, route)
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }
}
