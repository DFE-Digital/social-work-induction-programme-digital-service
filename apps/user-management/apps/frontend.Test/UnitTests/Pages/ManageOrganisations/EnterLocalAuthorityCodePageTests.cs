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

public class EnterLocalAuthorityCodePageTests : ManageOrganisationsPageTestBase<EnterLocalAuthorityCode>
{
    private EnterLocalAuthorityCode Sut { get; }

    public EnterLocalAuthorityCodePageTests()
    {
        Sut = new EnterLocalAuthorityCode(
            MockCreateOrganisationJourneyService.Object,
            MockOrganisationService.Object,
            new FakeLinkGenerator(),
            new EnterLocalAuthorityCodeValidator()
            );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var localAuthorityCodeInJourney = new Faker().Random.Int();
        MockCreateOrganisationJourneyService.Setup(x => x.GetLocalAuthorityCode()).Returns(localAuthorityCodeInJourney);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.LocalAuthorityCode.Should().Be(localAuthorityCodeInJourney);
        Sut.BackLinkPath.Should().Be("/manage-organisations");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetNew_WhenCalled_ResetsModelAndRedirectsToEnterLocalAuthorityCode()
    {
        // Act
        var result = Sut.OnGetNew();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Should().NotBeNull();
        result.Url.Should().Be("/manage-organisations/enter-local-authority-code");

        MockCreateOrganisationJourneyService.Verify(x => x.ResetCreateOrganisationJourneyModel(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var localAuthorityCodeInJourney = new Faker().Random.Int();
        MockCreateOrganisationJourneyService.Setup(x => x.GetLocalAuthorityCode()).Returns(localAuthorityCodeInJourney);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        Sut.LocalAuthorityCode.Should().Be(localAuthorityCodeInJourney);
        Sut.BackLinkPath.Should().Be("/manage-organisations/check-your-answers");
        result.Should().BeOfType<PageResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithEmptyLACode_ReturnsValidationErrors()
    {
        // Arrange
        Sut.LocalAuthorityCode = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("LocalAuthorityCode");
        modelState["LocalAuthorityCode"]!.Errors.Count.Should().Be(1);
        modelState["LocalAuthorityCode"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter a local authority code");

        Sut.BackLinkPath.Should().Be("/manage-organisations");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidLACode_ReturnsValidationErrors()
    {
        // Arrange
        Sut.LocalAuthorityCode = 1000; // Invalid LA code (should be 3 digits)

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("LocalAuthorityCode");
        modelState["LocalAuthorityCode"]!.Errors.Count.Should().Be(1);
        modelState["LocalAuthorityCode"]!.Errors[0].ErrorMessage.Should()
            .Be("The local authority code must be three numbers");

        Sut.BackLinkPath.Should().Be("/manage-organisations");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenModelBinderError_ReturnsCustomValidationErrorMessage()
    {
        // Arrange
        Sut.ModelState.AddModelError(nameof(EnterLocalAuthorityCode.LocalAuthorityCode), "Binder error");

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("LocalAuthorityCode");
        modelState["LocalAuthorityCode"]!.Errors.Count.Should().Be(1);
        modelState["LocalAuthorityCode"]!.Errors[0].ErrorMessage.Should()
            .Be("The local authority code must only include numbers");

        Sut.BackLinkPath.Should().Be("/manage-organisations");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithLACode_SavesLACodeAndRedirectsUser()
    {
        // Arrange
        var localAuthorityCode =  new Faker().Random.Int(100,999);
        Sut.LocalAuthorityCode = localAuthorityCode;

        var organisation = OrganisationBuilder.Build();

        MockOrganisationService
            .Setup(x => x.GetByLocalAuthorityCodeAsync(localAuthorityCode))
            .ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/confirm-organisation-details");

        MockOrganisationService.Verify(x => x.GetByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenLACodeNotFound_ReturnsValidationError()
    {
        // Arrange
        var localAuthorityCode =  new Faker().Random.Int(100,999);
        Sut.LocalAuthorityCode = localAuthorityCode;

        var organisation = OrganisationBuilder.Build();

        MockOrganisationService
            .Setup(x => x.GetByLocalAuthorityCodeAsync(localAuthorityCode))
            .ReturnsAsync((Organisation?)null);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("LocalAuthorityCode");
        modelState["LocalAuthorityCode"]!.Errors.Count.Should().Be(1);
        modelState["LocalAuthorityCode"]!.Errors[0].ErrorMessage.Should()
            .Be("The code you have entered is not associated with a local authority");

        Sut.BackLinkPath.Should().Be("/manage-organisations");

        MockOrganisationService.Verify(x => x.GetByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalledWithLACode_SavesLACodeAndRedirectsUser()
    {
        // Arrange
        var localAuthorityCode =  new Faker().Random.Int(100,999);
        Sut.LocalAuthorityCode = localAuthorityCode;

        var organisation = OrganisationBuilder.Build();

        MockOrganisationService
            .Setup(x => x.GetByLocalAuthorityCodeAsync(localAuthorityCode))
            .ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnPostChangeAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/check-your-answers");

        Sut.FromChangeLink.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-organisations/check-your-answers");

        MockOrganisationService.Verify(x => x.GetByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
