using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
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
    public EditPrimaryCoordinatorPageTests()
    {
        Sut = new EditPrimaryCoordinator(MockEditOrganisationJourneyService.Object, new FakeLinkGenerator(), new AccountDetailsValidator(MockAccountService.Object, MockAuthServiceClient.Object));
    }

    private EditPrimaryCoordinator Sut { get; }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
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
        var account = AccountBuilder.Build();
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
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var organisation = OrganisationBuilder.Build();

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);

        Sut.PrimaryCoordinator = accountDetails;

        MockEditOrganisationJourneyService.Setup(x => x.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, MoqHelpers.ShouldBeEquivalentTo(accountDetails)));

        // Act
        var result = await Sut.OnPostAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/check-your-answers/{organisation.OrganisationId!.Value}?handler=Edit");

        MockEditOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostReplaceAsync_WhenCalledWithValidData_RedirectsToConfirmDetails()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.PrimaryCoordinator = accountDetails;

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnPostReplaceAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/check-your-answers/{organisation.OrganisationId!.Value}?handler=Replace");

        Sut.IsReplace.Should().BeTrue();

        MockEditOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToAddCoordinatorDetails()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);
        var organisation = OrganisationBuilder.Build();

        Sut.PrimaryCoordinator = new AccountDetails
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };

        MockEditOrganisationJourneyService.Setup(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnPostAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(3);
        modelStateKeys.Should().Contain("PrimaryCoordinator.FirstName");
        modelState["PrimaryCoordinator.FirstName"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.FirstName"]!.Errors[0].ErrorMessage.Should().Be("Enter a first name");

        modelStateKeys.Should().Contain("PrimaryCoordinator.LastName");
        modelState["PrimaryCoordinator.LastName"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.LastName"]!.Errors[0].ErrorMessage.Should().Be("Enter a last name");

        modelStateKeys.Should().Contain("PrimaryCoordinator.Email");
        modelState["PrimaryCoordinator.Email"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostReplaceChangeAsync_WhenCalledWithValidData_RedirectsToConfirmDetails()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.PrimaryCoordinator = accountDetails;

        MockEditOrganisationJourneyService.Setup(x => x.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, MoqHelpers.ShouldBeEquivalentTo(accountDetails)));
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnPostReplaceChangeAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/check-your-answers/{organisation.OrganisationId!.Value}?handler=Replace");

        MockEditOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenEmailUnchanged_IgnoresUniquenessAndRedirects()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);
        accountDetails.Email = "Test@Test.com";
        var postedEmail = "  test@test.com  ";

        Sut.PrimaryCoordinator = accountDetails;
        Sut.PrimaryCoordinator.Email = postedEmail;

        MockEditOrganisationJourneyService.Setup(x => x.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, MoqHelpers.ShouldBeEquivalentTo(accountDetails)));
        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnPostAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        MockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(It.IsAny<string>()), Times.Never);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenEmailChanged_ChecksUniqueness_AndReturnsErrorWhenNotUnique()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        var postedAccount = AccountDetails.FromAccount(account);
        var postedEmail = "different@test.com";
        postedAccount.Email = postedEmail;

        Sut.PrimaryCoordinator = postedAccount;

        MockEditOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value)).ReturnsAsync(accountDetails);
        MockAccountService.Setup(x => x.CheckEmailExistsAsync(postedEmail)).ReturnsAsync(true);

        // Act
        var result = await Sut.OnPostAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("PrimaryCoordinator.Email");
        modelState["PrimaryCoordinator.Email"]!.Errors.Count.Should().Be(1);
        modelState["PrimaryCoordinator.Email"]!.Errors[0].ErrorMessage.Should().Be("The email address entered belongs to an existing user");

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value), Times.Once);
        MockAccountService.Verify(x => x.CheckEmailExistsAsync(postedEmail), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenPrimaryCoordinatorMissing_ReturnsBadRequest()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();

        MockEditOrganisationJourneyService
            .Setup(x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value))
            .ReturnsAsync((AccountDetails?)null);

        // Act
        var result = await Sut.OnPostAsync(organisation.OrganisationId!.Value);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        MockEditOrganisationJourneyService.Verify(
            x => x.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }
}
