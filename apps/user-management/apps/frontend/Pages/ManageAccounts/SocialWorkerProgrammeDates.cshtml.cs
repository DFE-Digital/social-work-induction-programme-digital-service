using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
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

        var accountDetails = createAccountJourneyService.GetAccountDetails();
        accountDetails!.ProgrammeStartDate = ProgrammeStartDate;
        accountDetails!.ProgrammeEndDate = ProgrammeEndDate;
        createAccountJourneyService.SetAccountDetails(accountDetails);

        return Redirect(linkGenerator.ConfirmAccountDetails());
    }
}
