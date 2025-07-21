using Bogus;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
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
        // TODO update error message once design is ready
        modelState["LocalAuthorityCode"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter the local authority code in full. (error message TBC)");

        Sut.BackLinkPath.Should().Be("/manage-organisations");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithLACode_SavesLACodeAndRedirectsUser()
    {
        // Arrange
        var localAuthorityCode =  new Faker().Random.Int();
        Sut.LocalAuthorityCode = localAuthorityCode;

        var organisation = OrganisationBuilder.Build();

        MockOrganisationService
            .Setup(x => x.GetByLocalAuthorityCode(localAuthorityCode))
            .Returns(organisation);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/confirm-organisation-details");

        MockOrganisationService.Verify(x => x.GetByLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalledWithLACode_SavesLACodeAndRedirectsUser()
    {
        // Arrange
        var localAuthorityCode =  new Faker().Random.Int();
        Sut.LocalAuthorityCode = localAuthorityCode;

        var organisation = OrganisationBuilder.Build();

        MockOrganisationService
            .Setup(x => x.GetByLocalAuthorityCode(localAuthorityCode))
            .Returns(organisation);

        // Act
        var result = await Sut.OnPostChangeAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/check-your-answers");

        Sut.FromChangeLink.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-organisations/check-your-answers");

        MockOrganisationService.Verify(x => x.GetByLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
