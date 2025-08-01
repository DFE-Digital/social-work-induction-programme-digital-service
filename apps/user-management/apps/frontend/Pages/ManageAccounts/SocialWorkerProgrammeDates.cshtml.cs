using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Social Worker Programme Dates View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class SocialWorkerProgrammeDates(
    ICreateAccountJourneyService createAccountJourneyService,
    IEditAccountJourneyService editAccountJourneyService,
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

    [BindProperty]
    public Guid? Id { get; set; }

    public async Task<IActionResult> OnGetAsync([FromQuery] Guid? id = null)
    {
        if (id.HasValue)
        {
            return await OnGetUpdateAsync(id.Value);
        }

        BackLinkPath = linkGenerator.ManageAccount.AddAccountDetails();

        var retrievedStartDate = createAccountJourneyService.GetProgrammeStartDate();
        ProgrammeStartDate = retrievedStartDate.HasValue
            ? new YearMonth(retrievedStartDate.Value.Year, retrievedStartDate.Value.Month)
            : null;

        var retrievedEndDate = createAccountJourneyService.GetProgrammeEndDate();
        ProgrammeEndDate = retrievedEndDate.HasValue
            ? new YearMonth(retrievedEndDate.Value.Year, retrievedEndDate.Value.Month)
            : null;

        return Page();
    }

    private async Task<IActionResult> OnGetUpdateAsync(Guid id)
    {
        Id = id;
        BackLinkPath = linkGenerator.ManageAccount.ViewAccountDetails(id);

        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (accountDetails?.ProgrammeStartDate is null || accountDetails.ProgrammeEndDate is null)
        {
            return NotFound();
        }

        ProgrammeStartDate = new YearMonth(
            accountDetails.ProgrammeStartDate.Value.Year,
            accountDetails.ProgrammeStartDate.Value.Month
        );
        ProgrammeEndDate = new YearMonth(
            accountDetails.ProgrammeEndDate.Value.Year,
            accountDetails.ProgrammeEndDate.Value.Month
        );

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
            BackLinkPath = Id.HasValue ? linkGenerator.ManageAccount.ViewAccountDetails(Id.Value) : linkGenerator.ManageAccount.AddAccountDetails();
            return Page();
        }

        if (Id.HasValue)
        {
            return await OnPostUpdateAsync(Id.Value);
        }

        if (ProgrammeStartDate.HasValue && ProgrammeEndDate.HasValue)
        {
            var dateOnlyStartDate = new DateOnly(ProgrammeStartDate.Value.Year, ProgrammeStartDate.Value.Month, 1);
            var dateOnlyEndDate = new DateOnly(ProgrammeEndDate.Value.Year, ProgrammeEndDate.Value.Month, 1);

            createAccountJourneyService.SetProgrammeStartDate(dateOnlyStartDate);
            createAccountJourneyService.SetProgrammeEndDate(dateOnlyEndDate);
        }

        return Redirect(linkGenerator.ManageAccount.ConfirmAccountDetails());
    }

    private async Task<IActionResult> OnPostUpdateAsync(Guid id)
    {
        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (Id.HasValue == false || accountDetails is null || !ProgrammeStartDate.HasValue || !ProgrammeEndDate.HasValue)
        {
            return Page();
        }

        var dateOnlyStartDate = new DateOnly(ProgrammeStartDate.Value.Year, ProgrammeStartDate.Value.Month, 1);
        var dateOnlyEndDate = new DateOnly(ProgrammeEndDate.Value.Year, ProgrammeEndDate.Value.Month, 1);

        accountDetails.ProgrammeStartDate = dateOnlyStartDate;
        accountDetails.ProgrammeEndDate = dateOnlyEndDate;

        await editAccountJourneyService.SetAccountDetailsAsync(id, accountDetails);

        return Redirect(linkGenerator.ManageAccount.ConfirmAccountDetailsUpdate(Id.Value));
    }
}
