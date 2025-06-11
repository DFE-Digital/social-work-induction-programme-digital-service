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

        var accountDetails = createAccountJourneyService.GetAccountDetails();

        ProgrammeStartDate = (accountDetails?.ProgrammeStartDate.HasValue ?? false)
            ? new YearMonth(
                accountDetails.ProgrammeStartDate.Value.Year,
                accountDetails.ProgrammeStartDate.Value.Month
            )
            : null;
        ProgrammeEndDate = (accountDetails?.ProgrammeEndDate.HasValue ?? false)
            ? new YearMonth(
                accountDetails.ProgrammeEndDate.Value.Year,
                accountDetails.ProgrammeEndDate.Value.Month
            )
            : null;

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
            //dates will be stored as the month and year entered, defaulted to day 1 of the month
            var accountDetails = createAccountJourneyService.GetAccountDetails();
            accountDetails!.ProgrammeStartDate =
                new DateOnly(ProgrammeStartDate.Value.Year, ProgrammeStartDate.Value.Month, 1);
            accountDetails!.ProgrammeEndDate =
                new DateOnly(ProgrammeEndDate.Value.Year, ProgrammeEndDate.Value.Month, 1);
            createAccountJourneyService.SetAccountDetails(accountDetails);
        }

        return Redirect(linkGenerator.ConfirmAccountDetails());
    }
}
