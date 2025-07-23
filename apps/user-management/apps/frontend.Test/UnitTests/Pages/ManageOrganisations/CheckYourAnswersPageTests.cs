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
            MockManageOrganisationJourneyService.Object,
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
        MockManageOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);
        MockManageOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountDetails()).Returns(primaryCoordinator);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.Organisation.Should().BeEquivalentTo(organisation);
        Sut.BackLinkPath.Should().Be("/manage-organisations/add-primary-coordinator");
        result.Should().BeOfType<PageResult>();

        MockManageOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockManageOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalled_RedirectsUser()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);

        MockManageOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);
        MockManageOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountDetails()).Returns(primaryCoordinator);

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

        MockManageOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockManageOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);
        MockManageOrganisationJourneyService.Verify(x => x.CompleteJourneyAsync(), Times.Once);

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

        MockManageOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockManageOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
