using Bogus;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;
using FluentAssertions;
using FluentValidation;
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
            MockAuthServiceClient.Object,
            new FakeLinkGenerator(),
            new EnterLocalAuthorityCodeValidator(MockOrganisationService.Object)
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var localAuthorityCodeInJourney = new Faker().Random.Int();
        var localAuthorityCodeString = localAuthorityCodeInJourney.ToString();
        MockCreateOrganisationJourneyService.Setup(x => x.GetLocalAuthorityCode()).Returns(localAuthorityCodeInJourney);

        // Act
        var result = Sut.OnGet();

        // Assert
        Sut.LocalAuthorityCode.Should().Be(localAuthorityCodeString);
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
        var localAuthorityCodeString = localAuthorityCodeInJourney.ToString();
        MockCreateOrganisationJourneyService.Setup(x => x.GetLocalAuthorityCode()).Returns(localAuthorityCodeInJourney);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        Sut.LocalAuthorityCode.Should().Be(localAuthorityCodeString);
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

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidLACode_ReturnsValidationErrors()
    {
        // Arrange
        Sut.LocalAuthorityCode = "1000"; // Invalid LA code (should be 3 digits)

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

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidLACodeStartingWithZero_ReturnsValidationErrors()
    {
        // Arrange
        Sut.LocalAuthorityCode = "012"; // Invalid LA code (should be 3 digits, not starting with zero)

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

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidLACodeString_ReturnsValidationErrors()
    {
        // Arrange
        Sut.LocalAuthorityCode = "invalid-la-code";

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

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithLACode_SavesLACodeAndRedirectsUser()
    {
        // Arrange
        var localAuthorityCode = new Faker().Random.Int(100, 999);
        Sut.LocalAuthorityCode = localAuthorityCode.ToString();

        var organisation = OrganisationBuilder.Build();

        MockAuthServiceClient
            .Setup(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode))
            .ReturnsAsync(organisation);
        MockOrganisationService
            .Setup(x => x.ExistsByLocalAuthorityCodeAsync(localAuthorityCode))
            .ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-organisations/confirm-organisation-details");

        MockAuthServiceClient.Verify(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        MockOrganisationService.Verify(x => x.ExistsByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenLACodeNotFound_ReturnsValidationError()
    {
        // Arrange
        var localAuthorityCode = new Faker().Random.Int(100, 999);
        Sut.LocalAuthorityCode = localAuthorityCode.ToString();

        var organisation = OrganisationBuilder.Build();

        MockAuthServiceClient
            .Setup(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode))
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

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        MockOrganisationService.Verify(x => x.ExistsByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockAuthServiceClient.Verify(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalledWithLACode_SavesLACodeAndRedirectsUser()
    {
        // Arrange
        var localAuthorityCode = new Faker().Random.Int(100, 999);
        Sut.LocalAuthorityCode = localAuthorityCode.ToString();

        var organisation = OrganisationBuilder.Build();

        MockAuthServiceClient
            .Setup(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode))
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

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        MockOrganisationService.Verify(x => x.ExistsByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockAuthServiceClient.Verify(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCannotParseInt_ReturnsValidationError()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<EnterLocalAuthorityCode>>();
        mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<EnterLocalAuthorityCode>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var sut = new EnterLocalAuthorityCode(
            MockCreateOrganisationJourneyService.Object,
            MockAuthServiceClient.Object,
            new FakeLinkGenerator(),
            mockValidator.Object
        )
        {
            LocalAuthorityCode = "not-an-int"
        };

        // Act
        var result = await sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        var error = sut.ModelState[nameof(sut.LocalAuthorityCode)]!.Errors[0].ErrorMessage;
        error.Should().Be("The local authority code must only include numbers");
    }

    [Fact]
    public async Task OnPostAsync_WhenLocalAuthorityCodeAlreadyExists_ReturnsUniquenessError()
    {
        // Arrange
        var localAuthorityCode = new Faker().Random.Int(100, 999);
        Sut.LocalAuthorityCode = localAuthorityCode.ToString();

        MockOrganisationService
            .Setup(x => x.ExistsByLocalAuthorityCodeAsync(localAuthorityCode))
            .ReturnsAsync(true);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        var error = Sut.ModelState[nameof(Sut.LocalAuthorityCode)]!.Errors[0].ErrorMessage;
        error.Should().Be("An organisation with this local authority code already exists");

        MockOrganisationService.Verify(x => x.ExistsByLocalAuthorityCodeAsync(localAuthorityCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        MockAuthServiceClient.Verify(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(localAuthorityCode), Times.Never);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(It.IsAny<int>()), Times.Never);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCodeUnchanged_SkipsUniquenessCheck()
    {
        // Arrange
        var existingCode = new Faker().Random.Int(100, 999);
        MockCreateOrganisationJourneyService.Setup(x => x.GetLocalAuthorityCode()).Returns(existingCode);
        Sut.LocalAuthorityCode = existingCode.ToString();

        var organisation = OrganisationBuilder.Build();
        MockAuthServiceClient
            .Setup(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(existingCode))
            .ReturnsAsync(organisation);

        // Act
        var result = await Sut.OnPostChangeAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        MockCreateOrganisationJourneyService.Verify(x => x.GetLocalAuthorityCode(), Times.Once);
        MockOrganisationService.Verify(x => x.ExistsByLocalAuthorityCodeAsync(existingCode), Times.Never);
        MockAuthServiceClient.Verify(x => x.LocalAuthority.GetByLocalAuthorityCodeAsync(existingCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetLocalAuthorityCode(existingCode), Times.Once);
        MockCreateOrganisationJourneyService.Verify(x => x.SetOrganisation(organisation), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
