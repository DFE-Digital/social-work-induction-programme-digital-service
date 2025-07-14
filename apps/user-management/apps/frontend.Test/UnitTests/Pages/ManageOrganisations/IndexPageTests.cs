using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using ManageOrganisationsIndex = Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public class IndexPageTests : ManageOrganisationsPageTestBase<ManageOrganisationsIndex>
{
    private ManageOrganisationsIndex Sut { get; }

    public IndexPageTests()
    {
        Sut = new ManageOrganisationsIndex(MockOrganisationService.Object);
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithAccountsSortedByCreatedAt()
    {
        // Arrange
        var expectedOrganisations = OrganisationBuilder.BuildMany(10);

        var paginationRequest = new PaginationRequest(0, 10);
        var paginationResponse = new PaginationResult<Organisation>
        {
            Records = expectedOrganisations,
            MetaData = new PaginationMetaData
            {
                Page = 1,
                PageSize = 5,
                PageCount = 2,
                TotalCount = 10,
                Links = new Dictionary<string, MetaDataLink>()
            }
        };

        MockOrganisationService
            .Setup(x => x.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)))
            .ReturnsAsync(paginationResponse);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Organisation.Should().NotBeEmpty();
        Sut.Organisation.Should().BeEquivalentTo(expectedOrganisations);
        Sut.Pagination.Should().BeEquivalentTo(paginationResponse.MetaData);

        MockOrganisationService.Verify(x => x.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
