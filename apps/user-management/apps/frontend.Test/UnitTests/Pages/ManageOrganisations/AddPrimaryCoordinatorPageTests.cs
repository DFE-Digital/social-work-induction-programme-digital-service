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

public class AddPrimaryCoordinatorPageTests : ManageOrganisationsPageTestBase<AddPrimaryCoordinator>
{
    public AddPrimaryCoordinatorPageTests()
    {
        Sut = new AddPrimaryCoordinator(
            MockCreateOrganisationJourneyService.Object,
            new AccountDetailsValidator(),
            new FakeLinkGenerator()
        );
    }

    private AddPrimaryCoordinator Sut { get; }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.AccountDetails.Should().BeEquivalentTo(accountDetails);
        Sut.BackLinkPath.Should().Be("/manage-organisations/enter-phone-number");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        Sut.AccountDetails.Should().BeEquivalentTo(accountDetails);
        Sut.BackLinkPath.Should().Be("/manage-organisations/check-your-answers");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetPrimaryCoordinatorAccountDetails(), Times.Once);
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

        Sut.AccountDetails = accountDetails;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/check-your-answers");

        MockCreateOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToAddCoordinatorDetails()
    {
        // Arrange
        Sut.AccountDetails = new AccountDetails
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(3);
        modelStateKeys.Should().Contain("AccountDetails.FirstName");
        modelState["AccountDetails.FirstName"]!.Errors.Count.Should().Be(1);
        modelState["AccountDetails.FirstName"]!.Errors[0].ErrorMessage.Should().Be("Enter a first name");

        modelStateKeys.Should().Contain("AccountDetails.LastName");
        modelState["AccountDetails.LastName"]!.Errors.Count.Should().Be(1);
        modelState["AccountDetails.LastName"]!.Errors[0].ErrorMessage.Should().Be("Enter a last name");

        modelStateKeys.Should().Contain("AccountDetails.Email");
        modelState["AccountDetails.Email"]!.Errors.Count.Should().Be(1);
        modelState["AccountDetails.Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalledWithValidData_RedirectsToConfirmDetails()
    {
        // Arrange
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.AccountDetails = accountDetails;

        // Act
        var result = await Sut.OnPostChangeAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/check-your-answers");

        Sut.BackLinkPath.Should().Be("/manage-organisations/check-your-answers");

        MockCreateOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }
}
