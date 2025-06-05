using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class SocialWorkerProgrammeDatesPageTests : ManageUsersPageTestBase<SocialWorkerProgrammeDates>
{
    private SocialWorkerProgrammeDates Sut { get; }

    public SocialWorkerProgrammeDatesPageTests()
    {
        Sut = new SocialWorkerProgrammeDates(
            new FakeLinkGenerator(),
            new SocialWorkerProgrammeDatesValidator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be("/manage-users/add-user-details");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDates_RedirectsToConfirmUserDetails()
    {
        // Arrange
        Sut.ProgrammeStartDate = new YearMonth(2020, 1);
        Sut.ProgrammeEndDate = new YearMonth(2030, 12);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/confirm-user-details");

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
                Sut.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
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

        Sut.BackLinkPath.Should().Be("/manage-users/add-user-details");

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

        Sut.BackLinkPath.Should().Be("/manage-users/add-user-details");

        VerifyAllNoOtherCalls();
    }
}
