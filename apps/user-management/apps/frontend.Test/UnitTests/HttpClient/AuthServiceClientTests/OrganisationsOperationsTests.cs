using System.Net;
using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public class OrganisationsOperationsTests : AuthServiceClientTestBase
{
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


    [Fact]
    public async Task ExistsByLocalAuthorityCode_SuccessfulRequest_ReturnsBoolean()
    {
        // Arrange
        var localAuthorityCode = new Faker().Random.Int(100, 999);
        var route = $"/api/Organisations/local-authority-code/{localAuthorityCode}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            true,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.Organisations.ExistsByLocalAuthorityCodeAsync(localAuthorityCode);

        // Assert
        response.Should().BeTrue();

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ExistsByLocalAuthorityCode_WhenErrorResponseReturned_ThrowsHttpRequestException()
    {
        // Arrange
        var localAuthorityCode = new Faker().Random.Int(100, 999);
        var route = $"/api/Organisations/local-authority-code/{localAuthorityCode}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.Organisations.ExistsByLocalAuthorityCodeAsync(localAuthorityCode)
        );

        // Assert
        exception.Message.Should().Be($"Failed to check if organisation exists with local authority code {localAuthorityCode}.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ExistsByLocalAuthorityCode_WhenResponseIsInvalidJson_ThrowsInvalidOperationException()
    {
        // Arrange
        var localAuthorityCode = new Faker().Random.Int(100, 999);
        var route = $"/api/Organisations/local-authority-code/{localAuthorityCode}";

        var (mockHttp, request) = GenerateMockClientWithRawResponse(
            HttpStatusCode.OK,
            HttpMethod.Get,
            "invalid-json",
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.Organisations.ExistsByLocalAuthorityCodeAsync(localAuthorityCode)
        );

        // Assert
        ex.Message.Should().Be("Failed to check if organisation exists.");
        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
