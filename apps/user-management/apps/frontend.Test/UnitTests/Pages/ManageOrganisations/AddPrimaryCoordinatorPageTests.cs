using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using Dfe.Sww.Ecf.Frontend.Validation;
using Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public class AddPrimaryCoordinatorPageTests : ManageOrganisationsPageTestBase<AddPrimaryCoordinator>
{
    private AddPrimaryCoordinator Sut { get; }

    public AddPrimaryCoordinatorPageTests()
    {
        Sut = new AddPrimaryCoordinator(
            MockCreateOrganisationJourneyService.Object,
            new AccountDetailsValidator(),
            new FakeLinkGenerator()
            );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.WithPhoneNumber().Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateOrganisationJourneyService.Setup(x => x.GetPrimaryCoordinatorAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.FirstName.Should().Be(account.FirstName);
        Sut.MiddleNames.Should().Be(account.MiddleNames);
        Sut.LastName.Should().Be(account.LastName);
        Sut.Email.Should().Be(account.Email);
        Sut.PhoneNumber.Should().Be(account.PhoneNumber);
        Sut.BackLinkPath.Should().Be("/manage-organisations/confirm-organisation-details");
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
            .WithPhoneNumber()
            .WithPhoneNumberRequired(true)
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.FirstName = accountDetails.FirstName;
        Sut.MiddleNames = accountDetails.MiddleNames;
        Sut.LastName = accountDetails.LastName;
        Sut.Email = accountDetails.Email;
        Sut.PhoneNumber = accountDetails.PhoneNumber;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/confirm-organisation-details");

        MockCreateOrganisationJourneyService.Verify(
            x => x.SetPrimaryCoordinatorAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToAddCoordinatorDetails()
    {
        // Arrange
        Sut.FirstName = string.Empty;
        Sut.LastName = string.Empty;
        Sut.Email = string.Empty;
        Sut.PhoneNumber = string.Empty;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(4);
        modelStateKeys.Should().Contain("FirstName");
        modelState["FirstName"]!.Errors.Count.Should().Be(1);
        modelState["FirstName"]!.Errors[0].ErrorMessage.Should().Be("Enter a first name");

        modelStateKeys.Should().Contain("LastName");
        modelState["LastName"]!.Errors.Count.Should().Be(1);
        modelState["LastName"]!.Errors[0].ErrorMessage.Should().Be("Enter a last name");

        modelStateKeys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email address");

        modelStateKeys.Should().Contain("PhoneNumber");
        modelState["PhoneNumber"]!.Errors.Count.Should().Be(1);
        modelState["PhoneNumber"]!.Errors[0].ErrorMessage.Should().Be("Enter a phone number");

        VerifyAllNoOtherCalls();
    }
}
