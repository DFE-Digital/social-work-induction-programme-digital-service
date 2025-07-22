using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public class OrganisationDetailsPageTests : ManageOrganisationsPageTestBase<OrganisationDetails>
{
    private OrganisationDetails Sut { get; }

    public OrganisationDetailsPageTests()
    {
        Sut = new OrganisationDetails(
            MockManageOrganisationJourneyService.Object,
            MockOrganisationService.Object,
            MockAccountService.Object,
            new FakeLinkGenerator()
        );
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var organisationId = organisation.OrganisationId ?? Guid.Empty;
        var primaryCoordinatorId = organisation.PrimaryCoordinatorId ?? Guid.Empty;
        var primaryCoordinator = AccountBuilder.WithId(primaryCoordinatorId).Build();

        MockOrganisationService.Setup(x => x.GetByIdAsync(organisationId)).ReturnsAsync(organisation);
        MockAccountService.Setup(x => x.GetByIdAsync(primaryCoordinatorId)).ReturnsAsync(primaryCoordinator);

        // Act
        var result = await Sut.OnGetAsync(organisationId);

        // Assert
        Sut.Organisation.Should().BeEquivalentTo(organisation);
        Sut.PrimaryCoordinator.Should().BeEquivalentTo(primaryCoordinator);
        Sut.BackLinkPath.Should().Be("/manage-organisations");
        result.Should().BeOfType<PageResult>();

        MockOrganisationService.Verify(x => x.GetByIdAsync(organisationId), Times.Once);
        MockAccountService.Verify(x => x.GetByIdAsync(primaryCoordinatorId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetAsync_WhenCalledAndNoOrganisationFound_ReturnsNotFoundResult()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockOrganisationService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Organisation?)null);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockOrganisationService.Verify(x => x.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetAsync_WhenCalledAndPrimaryCoordinatorIdIsNull_ReturnsNotFoundResult()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var organisationId = organisation.OrganisationId ?? Guid.Empty;
        organisation.PrimaryCoordinatorId = null;

        MockOrganisationService.Setup(x => x.GetByIdAsync(organisationId)).ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnGetAsync(organisationId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockOrganisationService.Verify(x => x.GetByIdAsync(organisationId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetAsync_WhenCalledAndPrimaryCoordinatorIsNull_ReturnsNotFoundResult()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var organisationId = organisation.OrganisationId ?? Guid.Empty;
        var primaryCoordinatorId = organisation.PrimaryCoordinatorId ?? Guid.Empty;

        MockOrganisationService.Setup(x => x.GetByIdAsync(organisationId)).ReturnsAsync(organisation);
        MockAccountService.Setup(x => x.GetByIdAsync(primaryCoordinatorId)).ReturnsAsync((Account?)null);

        // Act
        var result = await Sut.OnGetAsync(organisationId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockOrganisationService.Verify(x => x.GetByIdAsync(organisationId), Times.Once);
        MockAccountService.Verify(x => x.GetByIdAsync(primaryCoordinatorId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetNew_WhenCalled_ResetsModelAndRedirectsToOrganisationDetails()
    {
        // Act
        var result = Sut.OnGetNew(Guid.Empty);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Should().NotBeNull();
        result.Url.Should().Be($"/manage-organisations/organisation-details/{Guid.Empty}");

        MockManageOrganisationJourneyService.Verify(x => x.ResetOrganisationJourneyModel(), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
