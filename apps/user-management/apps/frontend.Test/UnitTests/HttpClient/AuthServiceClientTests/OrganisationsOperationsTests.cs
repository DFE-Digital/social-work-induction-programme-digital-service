using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Options;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;
using Person = Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Person;

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
        var organisations = new List<Organisation>
        {
            new()
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "test org",
                ExternalOrganisationId = 2,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                LocalAuthorityCode = 123,
                Type = OrganisationType.LocalAuthority
            }
        };

        var paginationRequest = new PaginationRequest(0, 10);
        var paginationResponse = new PaginationResult<Organisation>
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
    public async Task GetAll_WhenErrorResponseReturned_ReturnsNull()
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
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.Organisations.GetAllAsync(paginationRequest)
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
