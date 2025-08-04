using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public class CheckYourAnswersPageTests : ManageOrganisationsPageTestBase<CheckYourAnswers>
{
    private CheckYourAnswers Sut { get; }

    public CheckYourAnswersPageTests()
    {
        Sut = new CheckYourAnswers(
            MockCreateOrganisationJourneyService.Object,
            MockEditOrganisationJourneyService.Object,
            new FakeLinkGenerator()
        )
        {
            TempData = TempData
        };
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);
        MockCreateOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountDetails()).Returns(primaryCoordinator);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.Organisation.Should().BeEquivalentTo(organisation);
        Sut.BackLinkPath.Should().Be("/manage-organisations/add-primary-coordinator");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetEditAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);
        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(primaryCoordinator);

        // Act
        var result = await Sut.OnGetEditAsync(organisation.OrganisationId!.Value);

        // Assert
        Sut.Organisation.Should().BeEquivalentTo(organisation);
        Sut.BackLinkPath.Should().Be($"/manage-organisations/edit-primary-coordinator/{organisation.OrganisationId!.Value}");
        result.Should().BeOfType<PageResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetReplaceAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);
        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(primaryCoordinator);

        // Act
        var result = await Sut.OnGetReplaceAsync(organisation.OrganisationId!.Value);

        // Assert
        Sut.Organisation.Should().BeEquivalentTo(organisation);
        Sut.BackLinkPath.Should().Be($"/manage-organisations/edit-primary-coordinator/{organisation.OrganisationId!.Value}?handler=Replace");
        Sut.IsReplace.Should().BeTrue();
        result.Should().BeOfType<PageResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalled_RedirectsUser()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);

        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);
        MockCreateOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountDetails()).Returns(primaryCoordinator);

        Sut.Organisation = organisation;
        Sut.PrimaryCoordinator = primaryCoordinator;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations");

        TempData["NotificationType"].Should().Be(NotificationBannerType.Success);
        TempData["NotificationHeader"].Should().Be($"{organisation.OrganisationName} has been added");
        TempData["NotificationMessage"].Should().Be($"An invitation email has been sent to {primaryCoordinator.FullName}, {primaryCoordinator.Email}");

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.CompleteJourneyAsync(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithNullValues_ReturnsBadRequest()
    {
        // Arrange
        Sut.Organisation = null;
        Sut.PrimaryCoordinator = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<BadRequestResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);

        VerifyAllNoOtherCalls();
    }

        [Fact]
    public async Task OnPostEditAsync_WhenCalled_RedirectsUser()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(primaryCoordinator);

        Sut.Organisation = organisation;
        Sut.PrimaryCoordinator = primaryCoordinator;

        // Act
        var result = await Sut.OnPostEditAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations");

        TempData["NotificationType"].Should().Be(NotificationBannerType.Success);
        TempData["NotificationHeader"].Should().Be($"{organisation.OrganisationName} has been updated");
        TempData["NotificationMessage"].Should().Be($"An invitation email has been sent to {primaryCoordinator.FullName}, {primaryCoordinator.Email}");

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.CompleteJourneyAsync(organisation.OrganisationId!.Value), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostEditAsync_WhenCalledWithNullValues_ReturnsBadRequest()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        Sut.Organisation = null;
        Sut.PrimaryCoordinator = null;

        // Act
        var result = await Sut.OnPostEditAsync(organisationId);

        // Assert
        result.Should().BeOfType<BadRequestResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisationId), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisationId), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
