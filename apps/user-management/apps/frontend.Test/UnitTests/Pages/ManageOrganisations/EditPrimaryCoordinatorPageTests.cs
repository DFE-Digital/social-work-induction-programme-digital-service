using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public class EditPrimaryCoordinatorPageTests : ManageOrganisationsPageTestBase<EditPrimaryCoordinator>
{
    private EditPrimaryCoordinator Sut { get; }

    public EditPrimaryCoordinatorPageTests()
    {
        Sut = new EditPrimaryCoordinator(
            MockEditOrganisationJourneyService.Object,
            new FakeLinkGenerator(),
            new AccountDetailsValidator()
            );
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.WithPhoneNumber("07123123123").Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var organisation = OrganisationBuilder.Build();

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnGetAsync(organisation.OrganisationId!.Value);

        // Assert
        Sut.OrganisationName.Should().Be(organisation.OrganisationName);
        Sut.PrimaryCoordinator.Should().BeEquivalentTo(accountDetails);
        Sut.BackLinkPath.Should().Be($"/manage-organisations/edit-primary-coordinator-change-type/{organisation.OrganisationId!.Value}");
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

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnGetReplaceAsync(organisation.OrganisationId!.Value);

        // Assert
        Sut.OrganisationName.Should().Be(organisation.OrganisationName);
        Sut.BackLinkPath.Should().Be($"/manage-organisations/edit-primary-coordinator-change-type/{organisation.OrganisationId!.Value}");
        Sut.IsReplace.Should().BeTrue();
        result.Should().BeOfType<PageResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetReplaceChangeAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.WithPhoneNumber("07123123123").Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var organisation = OrganisationBuilder.Build();

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnGetReplaceChangeAsync(organisation.OrganisationId!.Value);

        // Assert
        Sut.OrganisationName.Should().Be(organisation.OrganisationName);
        Sut.PrimaryCoordinator.Should().BeEquivalentTo(accountDetails);
        Sut.BackLinkPath.Should().Be($"/manage-organisations/edit-primary-coordinator-change-type/{organisation.OrganisationId!.Value}");
        result.Should().BeOfType<PageResult>();

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidData_RedirectsToConfirmDetails()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithPhoneNumber("07123123123")
            .WithPhoneNumberRequired(true)
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.PrimaryCoordinator = accountDetails;

        MockEditOrganisationJourneyService.Setup(x => x.SetPrimaryCoordinatorAccountAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(accountDetails)));

        // Act
        var result = await Sut.OnPostAsync(organisationId);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/check-your-answers/{organisationId}?handler=Edit");

        MockEditOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostReplaceAsync_WhenCalledWithValidData_RedirectsToConfirmDetails()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithPhoneNumber("07123123123")
            .WithPhoneNumberRequired(true)
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.PrimaryCoordinator = accountDetails;

        MockEditOrganisationJourneyService.Setup(x => x.SetPrimaryCoordinatorAccountAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(accountDetails)));

        // Act
        var result = await Sut.OnPostReplaceAsync(organisationId);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/check-your-answers/{organisationId}?handler=Replace");

        Sut.IsReplace.Should().BeTrue();

        MockEditOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToAddCoordinatorDetails()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        Sut.PrimaryCoordinator = new AccountDetails
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            PhoneNumber = string.Empty
        };

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value));

        // Act
        var result = await Sut.OnPostAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(4);
        modelStateKeys.Should().Contain("PrimaryCoordinator.FirstName");
        modelState["PrimaryCoordinator.FirstName"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.FirstName"]!.Errors[0].ErrorMessage.Should().Be("Enter a first name");

        modelStateKeys.Should().Contain("PrimaryCoordinator.LastName");
        modelState["PrimaryCoordinator.LastName"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.LastName"]!.Errors[0].ErrorMessage.Should().Be("Enter a last name");

        modelStateKeys.Should().Contain("PrimaryCoordinator.Email");
        modelState["PrimaryCoordinator.Email"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");

        modelStateKeys.Should().Contain("PrimaryCoordinator.PhoneNumber");
        modelState["PrimaryCoordinator.PhoneNumber"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.PhoneNumber"]!.Errors[0].ErrorMessage.Should().Be("Enter a phone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192");

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostReplaceChangeAsync_WhenCalledWithValidData_RedirectsToConfirmDetails()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithPhoneNumber("07123123123")
            .WithPhoneNumberRequired(true)
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.PrimaryCoordinator = accountDetails;

        MockEditOrganisationJourneyService.Setup(x => x.SetPrimaryCoordinatorAccountAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(accountDetails)));

        // Act
        var result = await Sut.OnPostReplaceChangeAsync(organisationId);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/check-your-answers/{organisationId}?handler=Replace");

        MockEditOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountAsync(organisationId, MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }
}
