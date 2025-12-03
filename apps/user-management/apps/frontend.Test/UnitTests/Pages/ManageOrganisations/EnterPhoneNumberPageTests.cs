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
            MockEditOrganisationJourneyService.Object,
            new FakeLinkGenerator(),
            new EnterPhoneNumberValidator()
            );
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.PhoneNumber.Should().Be(organisation.PhoneNumber);
        Sut.BackLinkPath.Should().Be("/manage-organisations/confirm-organisation-details");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetOrganisation(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetChangeAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        MockCreateOrganisationJourneyService.Setup(x => x.GetOrganisation()).Returns(organisation);

        // Act
        var result = await Sut.OnGetChangeAsync();

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
        redirectResult!.Url.Should().Be("/manage-organisations/check-your-answers?handler=FromPhoneNumberChange");

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

    [Fact]
    public async Task OnGetAsync_WhenCalledWithId_LoadsExistingPhoneNumberAndBackLinkToOrganisationDetails()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var id = organisation.OrganisationId!.Value;

        MockEditOrganisationJourneyService
            .Setup(x => x.GetOrganisationAsync(id))
            .ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.PhoneNumber.Should().Be(organisation.PhoneNumber);
        Sut.Id.Should().Be(id);
        Sut.BackLinkPath.Should().Be($"/manage-organisations/organisation-details/{id}");

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIdAndValidPhoneNumber_UpdatesOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var id = organisation.OrganisationId!.Value;

        Sut.Id = id;
        Sut.PhoneNumber = new Faker().Phone.PhoneNumber("+447### ######");

        MockEditOrganisationJourneyService
            .Setup(x => x.GetOrganisationAsync(id))
            .ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-organisations/check-your-answers/{id}?handler=EditPhoneNumber");

        organisation.PhoneNumber.Should().Be(Sut.PhoneNumber);

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(id), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.SetOrganisationAsync(id, organisation), Times.Once);
        MockEditOrganisationJourneyService.Verify(x => x.SetIsOrganisationUpdateAsync(id, true), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIdAndNoOrganisation_ThrowsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        Sut.Id = id;
        Sut.PhoneNumber = new Faker().Phone.PhoneNumber("+447### ######");

        MockEditOrganisationJourneyService
            .Setup(x => x.GetOrganisationAsync(id))
            .ReturnsAsync((Organisation?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.OnPostAsync());

        // Assert
        exception.Message.Should().Be("Organisation must be set before accessing this page.");

        MockEditOrganisationJourneyService.Verify(x => x.GetOrganisationAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
