using System.Net;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public class LocalAuthorityOperationsTests : AuthServiceClientTestBase
{
    [Fact]
    public async Task GetByLocalAuthorityCode_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        const int localAuthorityCode = 100;
        var route = $"/api/LocalAuthority/{localAuthorityCode}";
        var localAuthority = new LocalAuthorityDto()
        {
            OrganisationName = "test org",
            LocalAuthorityCode = localAuthorityCode,
            Region = "Test region"
        };
        var organisation = localAuthority.ToOrganisation();

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            localAuthority,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Organisation>();
        response.Should().BeEquivalentTo(organisation);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetByLocalAuthorityCode_WhenErrorResponseReturned_ThrowsHttpRequestException()
    {
        // Arrange
        const int localAuthorityCode = 100;
        var route = $"/api/LocalAuthority/{localAuthorityCode}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode)
        );

        // Assert
        exception.Message.Should().Be($"Failed to get organisation with local authority code {localAuthorityCode}.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetByLocalAuthorityCode_WhenNoOrganisationReturned_ThrowsException()
    {
        // Arrange
        const int localAuthorityCode = 100;
        var route = $"/api/LocalAuthority/{localAuthorityCode}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act & Assert
        await sut.Invoking(s => s.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to get local authority data.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetByLocalAuthorityCode_WhenNoContentResponseReturned_ReturnsNull()
    {
        // Arrange
        const int localAuthorityCode = 100;
        var route = $"/api/LocalAuthority/{localAuthorityCode}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.NoContent,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode);

        // Assert
        response.Should().BeNull();

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
