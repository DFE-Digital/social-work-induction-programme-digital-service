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
            MockEditOrganisationJourneyService.Object,
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
        var account = AccountBuilder.WithId(primaryCoordinatorId).Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisationId)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisationId)).ReturnsAsync(primaryCoordinator);

        // Act
        var result = await Sut.OnGetAsync(organisationId);

        // Assert
        Sut.Organisation.Should().BeEquivalentTo(organisation);
        Sut.PrimaryCoordinator.Should().BeEquivalentTo(primaryCoordinator);
        Sut.BackLinkPath.Should().Be("/manage-organisations");
        result.Should().BeOfType<PageResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisationId), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisationId), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.SetOrganisationAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(organisation)), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.SetPrimaryCoordinatorAccountAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(primaryCoordinator)), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetAsync_WhenCalledAndNoOrganisationFound_ReturnsNotFoundResult()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(id)).ReturnsAsync((Organisation?)null);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(id)).ReturnsAsync((AccountDetails?)null);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(id), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(id), Times.Once);
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

        MockEditOrganisationJourneyService.Verify(x => x.ResetEditOrganisationJourneyModel(Guid.Empty), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
