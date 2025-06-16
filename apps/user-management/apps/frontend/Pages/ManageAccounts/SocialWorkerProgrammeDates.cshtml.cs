using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Social Worker Programme Dates View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class SocialWorkerProgrammeDates(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<SocialWorkerProgrammeDates> validator) : BasePageModel
{
    [BindProperty]
    [DateInput(DateInputItemTypes.MonthAndYear, ErrorMessagePrefix = "Programme start date")]
    [Required(ErrorMessage = "Enter a programme start date")]
    public YearMonth? ProgrammeStartDate { get; set; }

    [BindProperty]
    [DateInput(DateInputItemTypes.MonthAndYear, ErrorMessagePrefix = "Programme end date")]
    [Required(ErrorMessage = "Enter an expected programme end date")]
    public YearMonth? ProgrammeEndDate { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.AddAccountDetails();

        DateOnly? retrievedStartDate = createAccountJourneyService.GetProgrammeStartDate();

        ProgrammeStartDate = retrievedStartDate.HasValue
            ? new YearMonth(retrievedStartDate.Value.Year, retrievedStartDate.Value.Month)
            : (YearMonth?)null;

        DateOnly? retrievedEndDate = createAccountJourneyService.GetProgrammeEndDate();

        ProgrammeEndDate = retrievedEndDate.HasValue
            ? new YearMonth(retrievedEndDate.Value.Year, retrievedEndDate.Value.Month)
            : (YearMonth?)null;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
        }

        if (!ModelState.IsValid || !result.IsValid)
        {
            BackLinkPath = linkGenerator.AddAccountDetails();
            return Page();
        }

        if (ProgrammeStartDate.HasValue && ProgrammeEndDate.HasValue)
        {
            var dateOnlyStartDate = new DateOnly(ProgrammeStartDate.Value.Year, ProgrammeStartDate.Value.Month, 1);
            var dateOnlyEndDate = new DateOnly(ProgrammeEndDate.Value.Year, ProgrammeEndDate.Value.Month, 1);

            createAccountJourneyService.SetProgrammeStartDate(dateOnlyStartDate);
            createAccountJourneyService.SetProgrammeEndDate(dateOnlyEndDate);
        }

        return Redirect(linkGenerator.ConfirmAccountDetails());
    }
}
