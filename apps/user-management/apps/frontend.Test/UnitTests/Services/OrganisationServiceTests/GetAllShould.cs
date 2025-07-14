using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.OrganisationServiceTests;

public class GetAllShould : OrganisationAccountServiceTestBase
{
    [Fact]
    public async Task WhenCalled_ReturnsAllOrganisations()
    {
        // Arrange
        var organisations = OrganisationBuilder.BuildMany(10);
        var organisationDtos = organisations.Select(x => Mapper.MapFromBo(x));

        var paginationRequest = new PaginationRequest(0, 10);
        var metaData = new PaginationMetaData
        {
            Page = 1,
            PageSize = 5,
            PageCount = 2,
            TotalCount = 10,
            Links = new Dictionary<string, MetaDataLink>()
        };
        var clientResponse = new PaginationResult<OrganisationDto>
        {
            Records = organisationDtos,
            MetaData = metaData
        };
        var paginationResponse = new PaginationResult<Organisation>
        {
            Records = organisations,
            MetaData = metaData
        };

        MockClient
            .Setup(x => x.Organisations.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)))
            .ReturnsAsync(clientResponse);

        // Act
        var response = await Sut.GetAllAsync(paginationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<PaginationResult<Organisation>>();
        response.Should().BeEquivalentTo(paginationResponse);

        MockClient.Verify(
            x => x.Organisations.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalledAndOrganisationClaimIsMissing_ThrowNullReferenceException()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(0, 10);

        MockClient.SetupMockHttpContextAccessorWithEmptyClaimsPrincipal();

        // Act & Assert
        await FluentActions.Awaiting(() => Sut.GetAllAsync(paginationRequest))
            .Should().ThrowAsync<NullReferenceException>();
    }
}
