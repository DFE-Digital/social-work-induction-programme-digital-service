using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public class EditPrimaryCoordinatorChangeTypePageTests : ManageOrganisationsPageTestBase<EditPrimaryCoordinatorChangeType>
{
    private EditPrimaryCoordinatorChangeType Sut { get; }

    public EditPrimaryCoordinatorChangeTypePageTests()
    {
        Sut = new EditPrimaryCoordinatorChangeType(
            MockEditOrganisationJourneyService.Object,
            new FakeLinkGenerator(),
            new EditPrimaryCoordinatorChangeTypeValidator()
        );
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var organisationId = organisation.OrganisationId ?? Guid.Empty;
        var primaryCoordinatorChangeType = PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator;

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisationId)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorChangeTypeAsync(organisationId)).ReturnsAsync(primaryCoordinatorChangeType);

        // Act
        var result = await Sut.OnGetAsync(organisationId);

        // Assert
        Sut.OrganisationName.Should().BeEquivalentTo(organisation.OrganisationName);
        Sut.BackLinkPath.Should().Be($"/manage-organisations/organisation-details/{organisationId}");
        result.Should().BeOfType<PageResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisationId), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorChangeTypeAsync(organisationId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDataAndUpdatingExisting_RedirectsToRelevantPage()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        Sut.ChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator;

        // Act
        var result = await Sut.OnPostAsync(organisationId);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/edit-primary-coordinator/{organisationId}");

        MockEditOrganisationJourneyService.Verify(x => x.SetPrimaryCoordinatorChangeTypeAsync(organisationId, Sut.ChangeType), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDataAndReplacingAccount_RedirectsToRelevantPage()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        Sut.ChangeType = PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator;

        // Act
        var result = await Sut.OnPostAsync(organisationId);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/edit-primary-coordinator/{organisationId}?handler=Replace");

        MockEditOrganisationJourneyService.Verify(x => x.SetPrimaryCoordinatorChangeTypeAsync(organisationId, Sut.ChangeType), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToSelectChangeType()
    {
        // Arrange
        Sut.ChangeType = null;
        var organisation = OrganisationBuilder.Build();
        var organisationId = organisation.OrganisationId ?? Guid.Empty;

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisationId)).ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnPostAsync(organisationId);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("ChangeType");
        modelState["ChangeType"]!.Errors.Count.Should().Be(1);
        modelState["ChangeType"]!.Errors[0].ErrorMessage.Should().Be("Select the type of change you are making to the primary coordinator");

        Sut.OrganisationName.Should().Be(organisation.OrganisationName);

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisationId), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
