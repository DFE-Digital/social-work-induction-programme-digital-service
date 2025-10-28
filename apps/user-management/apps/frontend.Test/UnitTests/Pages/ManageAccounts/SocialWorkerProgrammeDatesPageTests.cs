using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NodaTime;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SocialWorkerProgrammeDatesPageTests : ManageAccountsPageTestBase<SocialWorkerProgrammeDates>
{
    private SocialWorkerProgrammeDates Sut { get; }

    public SocialWorkerProgrammeDatesPageTests()
    {
        Sut = new SocialWorkerProgrammeDates(
            MockCreateAccountJourneyService.Object,
            MockEditAccountJourneyService.Object,
            new FakeLinkGenerator(),
            new SocialWorkerProgrammeDatesValidator()
        );
    }

    [Fact]
    public async Task OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeStartDate()).Returns(accountDetails.ProgrammeStartDate);
        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeEndDate()).Returns(accountDetails.ProgrammeEndDate);
        var expectedProgrammeStartDate = new YearMonth(
            accountDetails.ProgrammeStartDate!.Value.Year,
            accountDetails.ProgrammeStartDate.Value.Month
        );
        var expectedProgrammeEndDate = new YearMonth(
                accountDetails.ProgrammeEndDate!.Value.Year,
                accountDetails.ProgrammeEndDate.Value.Month
            );

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.ProgrammeStartDate.Should().Be(expectedProgrammeStartDate);
        Sut.ProgrammeEndDate.Should().Be(expectedProgrammeEndDate);
        Sut.BackLinkPath.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeStartDate(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeEndDate(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetUpdate_WhenCalled_LoadsTheView()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(id)).ReturnsAsync(accountDetails);
        var expectedProgrammeStartDate = new YearMonth(
            accountDetails.ProgrammeStartDate!.Value.Year,
            accountDetails.ProgrammeStartDate.Value.Month
        );
        var expectedProgrammeEndDate = new YearMonth(
            accountDetails.ProgrammeEndDate!.Value.Year,
            accountDetails.ProgrammeEndDate.Value.Month
        );

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.ProgrammeStartDate.Should().Be(expectedProgrammeStartDate);
        Sut.ProgrammeEndDate.Should().Be(expectedProgrammeEndDate);
        Sut.Id.Should().Be(id);
        Sut.BackLinkPath.Should().Be($"/manage-accounts/view-account-details/{id}");

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetUpdate_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(id)).ReturnsAsync((AccountDetails?)null);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        Sut.Id.Should().Be(id);
        Sut.BackLinkPath.Should().Be($"/manage-accounts/view-account-details/{id}");

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnGetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeStartDate()).Returns(accountDetails.ProgrammeStartDate);
        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeEndDate()).Returns(accountDetails.ProgrammeEndDate);
        var expectedProgrammeStartDate = new YearMonth(
            accountDetails.ProgrammeStartDate!.Value.Year,
            accountDetails.ProgrammeStartDate.Value.Month
        );
        var expectedProgrammeEndDate = new YearMonth(
            accountDetails.ProgrammeEndDate!.Value.Year,
            accountDetails.ProgrammeEndDate.Value.Month
        );

        // Act
        var result = await Sut.OnGetChangeAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.ProgrammeStartDate.Should().Be(expectedProgrammeStartDate);
        Sut.ProgrammeEndDate.Should().Be(expectedProgrammeEndDate);
        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");

        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeStartDate(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeEndDate(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDates_RedirectsToConfirmUserDetails()
    {
        // Arrange
        Sut.ProgrammeStartDate = new YearMonth(2020, 1);
        Sut.ProgrammeEndDate = new YearMonth(2050, 12);
        var expectedStartDate = new DateOnly(
            Sut.ProgrammeStartDate.Value.Year,
            Sut.ProgrammeStartDate.Value.Month,
            1);
        var expectedEndDate = new DateOnly(
            Sut.ProgrammeEndDate.Value.Year,
            Sut.ProgrammeEndDate.Value.Month,
            1);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/confirm-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetProgrammeStartDate(expectedStartDate), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetProgrammeEndDate(expectedEndDate), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostUpdateAsync_WhenCalledWithValidDates_RedirectsToConfirmUserDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = AccountBuilder.Build();
        var accountDetails = AccountDetails.FromAccount(account);

        Sut.Id = id;
        Sut.ProgrammeStartDate = new YearMonth(2020, 1);
        Sut.ProgrammeEndDate = new YearMonth(2050, 12);
        var expectedStartDate = new DateOnly(
            Sut.ProgrammeStartDate.Value.Year,
            Sut.ProgrammeStartDate.Value.Month,
            1);
        var expectedEndDate = new DateOnly(
            Sut.ProgrammeEndDate.Value.Year,
            Sut.ProgrammeEndDate.Value.Month,
            1);

        MockEditAccountJourneyService.Setup(x => x.GetAccountDetailsAsync(id)).ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"/manage-accounts/confirm-account-details/{id}?handler=Update");

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.SetAccountDetailsAsync(id, MoqHelpers.ShouldBeEquivalentTo(accountDetails)), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalledWithValidDates_RedirectsToConfirmUserDetails()
    {
        // Arrange
        Sut.ProgrammeStartDate = new YearMonth(2020, 1);
        Sut.ProgrammeEndDate = new YearMonth(2050, 12);
        var expectedStartDate = new DateOnly(
            Sut.ProgrammeStartDate.Value.Year,
            Sut.ProgrammeStartDate.Value.Month,
            1);
        var expectedEndDate = new DateOnly(
            Sut.ProgrammeEndDate.Value.Year,
            Sut.ProgrammeEndDate.Value.Month,
            1);

        // Act
        var result = await Sut.OnPostChangeAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/confirm-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetProgrammeStartDate(expectedStartDate), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetProgrammeEndDate(expectedEndDate), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithBlankDates_ReturnsErrors()
    {
        // Arrange
        Sut.ProgrammeStartDate = null;
        Sut.ProgrammeEndDate = null;

        // Simulate non-fluent validation model validation
        var validationContext = new ValidationContext(Sut, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(Sut, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                Sut.ModelState.AddModelError(memberName, validationResult.ErrorMessage!);
            }
        }

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(2);
        modelStateKeys.Should().Contain("ProgrammeStartDate");
        modelState["ProgrammeStartDate"]!.Errors.Count.Should().Be(1);
        modelState["ProgrammeStartDate"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter a programme start date");

        modelStateKeys.Should().Contain("ProgrammeEndDate");
        modelState["ProgrammeEndDate"]!.Errors.Count.Should().Be(1);
        modelState["ProgrammeEndDate"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter an expected programme end date");

        Sut.BackLinkPath.Should().Be("/manage-accounts/add-account-details");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostChangeAsync_WhenCalledWithBlankDates_ReturnsErrors()
    {
        // Arrange
        Sut.ProgrammeStartDate = null;
        Sut.ProgrammeEndDate = null;

        // Simulate non-fluent validation model validation
        var validationContext = new ValidationContext(Sut, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(Sut, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                Sut.ModelState.AddModelError(memberName, validationResult.ErrorMessage!);
            }
        }

        // Act
        var result = await Sut.OnPostChangeAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(2);
        modelStateKeys.Should().Contain("ProgrammeStartDate");
        modelState["ProgrammeStartDate"]!.Errors.Count.Should().Be(1);
        modelState["ProgrammeStartDate"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter a programme start date");

        modelStateKeys.Should().Contain("ProgrammeEndDate");
        modelState["ProgrammeEndDate"]!.Errors.Count.Should().Be(1);
        modelState["ProgrammeEndDate"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter an expected programme end date");

        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithPastEndDate_ReturnsError()
    {
        // Arrange
        Sut.ProgrammeStartDate = new YearMonth(2020, 1);
        Sut.ProgrammeEndDate = new YearMonth(2021, 1);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("ProgrammeEndDate");
        modelState["ProgrammeEndDate"]!.Errors.Count.Should().Be(1);
        modelState["ProgrammeEndDate"]!.Errors[0].ErrorMessage.Should()
            .Be("Expected programme end date must be in the future");

        Sut.BackLinkPath.Should().Be("/manage-accounts/add-account-details");

        VerifyAllNoOtherCalls();
    }
}
