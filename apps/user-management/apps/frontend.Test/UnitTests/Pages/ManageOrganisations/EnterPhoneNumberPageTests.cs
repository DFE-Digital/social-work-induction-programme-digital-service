using Bogus;
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

public class EnterPhoneNumberPageTests : ManageOrganisationsPageTestBase<EnterPhoneNumber>
{
    private EnterPhoneNumber Sut { get; }

    public EnterPhoneNumberPageTests()
    {
        Sut = new EnterPhoneNumber(
            MockCreateOrganisationJourneyService.Object,
            new FakeLinkGenerator(),
            new EnterPhoneNumberValidator()
            );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.PhoneNumber.Should().Be(organisation.PhoneNumber);
        Sut.BackLinkPath.Should().Be("/manage-organisations/confirm-organisation-details");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        Sut.PhoneNumber.Should().Be(organisation.PhoneNumber);
        Sut.BackLinkPath.Should().Be("/manage-organisations/check-your-answers");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithEmptyPhoneNumber_ReturnsValidationErrors()
    {
        // Arrange
        Sut.PhoneNumber = string.Empty;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("PhoneNumber");
        modelState["PhoneNumber"]!.Errors.Count.Should().Be(1);
        modelState["PhoneNumber"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter a phone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192");

        Sut.BackLinkPath.Should().Be("/manage-organisations/confirm-organisation-details");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithPhoneNumber_SavesPhoneNumberAndRedirectsUser()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        Sut.PhoneNumber = organisation.PhoneNumber;
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/add-primary-coordinator");

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalledWithPhoneNumber_SavesPhoneNumberAndRedirectsUser()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        Sut.PhoneNumber = organisation.PhoneNumber;
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);

        // Act
        var result = await Sut.OnPostChangeAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/check-your-answers");

        Sut.FromChangeLink.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-organisations/check-your-answers");

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithPhoneNumberAndNoOrganisation_ThrowsError()
    {
        // Arrange
        Sut.PhoneNumber = new Faker().Phone.PhoneNumber("+447### ######");
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns((Organisation?)null);

        // Act
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Sut.OnPostAsync()
        );

        // Assert
        actualException.Message.Should().Be("Organisation must be set before accessing this page.");

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);

        VerifyAllNoOtherCalls();

    }
}
