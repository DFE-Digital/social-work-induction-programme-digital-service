using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Options;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public class OrganisationsOperationsTests
{
    private readonly Mock<IOptions<AuthClientOptions>> _mockOptions;

    public OrganisationsOperationsTests()
    {
        _mockOptions = new();
    }

    [Fact]
    public async Task GetAll_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var route = "/api/Organisations";
        var organisations = new List<OrganisationDto>
        {
            new()
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "test org",
                ExternalOrganisationId = 2,
                LocalAuthorityCode = 123,
                Type = OrganisationType.LocalAuthority
            }
        };

        var paginationRequest = new PaginationRequest(0, 10);
        var paginationResponse = new PaginationResult<OrganisationDto>
        {
            Records = organisations,
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
        var response = await sut.Organisations.GetAllAsync(paginationRequest);

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
        var route = "/api/Organisations";

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
            () => sut.Organisations.GetAllAsync(paginationRequest)
        );

        // Assert
        exception.Message.Should().Be("Failed to get organisations.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var route = $"/api/Organisations/{organisationId}";
        var organisation = new Organisation
        {
            OrganisationId = organisationId,
            OrganisationName = "test org",
            ExternalOrganisationId = 2,
            LocalAuthorityCode = 123,
            Type = OrganisationType.LocalAuthority,
            Region = "Test region",
            PrimaryCoordinatorId = Guid.Empty
        };

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            organisation,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Organisations.GetByIdAsync(organisationId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<OrganisationDto>();
        response.Should().BeEquivalentTo(organisation);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_WhenErrorResponseReturned_ThrowsHttpRequestException()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var route = $"/api/Organisations/{organisationId}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.Organisations.GetByIdAsync(organisationId)
        );

        // Assert
        exception.Message.Should().Be($"Failed to get organisation with ID {organisationId}.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_WhenNoOrganisationReturned_ThrowsException()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var route = $"/api/Organisations/{organisationId}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act & Assert
        await sut.Invoking(s => s.Organisations.GetByIdAsync(organisationId))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to get organisation.");

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

        var claims = new List<Claim>
        {
            new Claim("organisation_id", Guid.NewGuid().ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var sut = new AuthServiceClient(client, mockHttpContextAccessor.Object);

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
