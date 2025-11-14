using System.Net;
using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;
using Person = Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Person;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public class AccountsOperationsTests : AuthServiceClientTestBase
{
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
    public async Task GetAll_WhenErrorResponseReturned_ThrowsHttpRequestException()
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
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.Accounts.GetAllAsync(paginationRequest)
        );

        // Assert
        exception.Message.Should().Be("Failed to get accounts.");

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
    public async Task GetById_WhenErrorResponseReturned_ThrowsHttpRequestException()
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
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.Accounts.GetByIdAsync(personId)
        );

        // Assert
        exception.Message.Should().Be($"Failed to get account with ID {personId}.");

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
    public async Task Create_WhenErrorResponseReturned_ThrowsHttpRequestException()
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
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.Accounts.CreateAsync(createRequest)
        );

        // Assert
        exception.Message.Should().Be("Failed to create account.");

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
    public async Task Update_WhenErrorResponseReturned_ThrowsHttpRequestException()
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
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.Accounts.UpdateAsync(updateRequest)
        );

        // Assert
        exception.Message.Should().Be("Failed to update account.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CheckEmailExists_SuccessfulRequest_ReturnsBoolean()
    {
        // Arrange
        var checkEmailRequest = new CheckEmailRequest
        {
            Email = "test@test.com"
        };

        const string route = "/api/Accounts/check";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Post,
            true,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Accounts.CheckEmailExistsAsync(checkEmailRequest);

        // Assert
        response.Should().BeTrue();

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CheckEmailExists_WhenErrorResponseReturned_ThrowsHttpRequestException()
    {
        // Arrange
        var checkEmailRequest = new CheckEmailRequest
        {
            Email = "test@test.com"
        };

        const string route = "/api/Accounts/check";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Post,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.Accounts.CheckEmailExistsAsync(checkEmailRequest)
        );

        // Assert
        exception.Message.Should().Be("Failed to check if email exists.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task Create_WhenResponseIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var createRequest = new CreatePersonRequest
        {
            FirstName = "Test",
            LastName = "Tester",
            SocialWorkEnglandNumber = "SW123",
            EmailAddress = "test@test.com",
            Status = AccountStatus.Active
        };

        const string route = "/api/Accounts/Create";

        var (mockHttp, request) = GenerateMockClientWithRawResponse(
            HttpStatusCode.OK,
            HttpMethod.Post,
            "null",
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.Accounts.CreateAsync(createRequest)
        );

        // Assert
        ex.Message.Should().Be("Failed to create account.");
        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CheckEmailExists_WhenResponseIsInvalidJson_ThrowsInvalidOperationException()
    {
        // Arrange
        var checkEmailRequest = new CheckEmailRequest
        {
            Email = "test@test.com"
        };

        const string route = "/api/Accounts/check";

        var (mockHttp, request) = GenerateMockClientWithRawResponse(
            HttpStatusCode.OK,
            HttpMethod.Post,
            "invalid-json",
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.Accounts.CheckEmailExistsAsync(checkEmailRequest)
        );

        // Assert
        ex.Message.Should().Be("Failed to check if email exists.");
        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
