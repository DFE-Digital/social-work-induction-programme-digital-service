using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EditAccountDetails(
    IEditAccountJourneyService editAccountJourneyService,
    IValidator<AccountDetails> validator,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    public Guid Id { get; set; }

    [BindProperty]
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    [BindProperty]
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    [BindProperty]
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    [BindProperty]
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }

    public bool IsStaff { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ViewAccountDetails(id);
        Id = id;

        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);

        FirstName = accountDetails.FirstName;
        LastName = accountDetails.LastName;
        Email = accountDetails.Email;

        var isSwe = SocialWorkEnglandRecord.TryParse(
            accountDetails.SocialWorkEnglandNumber,
            out var swe
        );
        SocialWorkEnglandNumber = isSwe ? swe?.GetNumber().ToString() : null;
        IsStaff = await editAccountJourneyService.GetIsStaffAsync(id) ?? false;

        return Page();
    }

    public async Task<IActionResult> OnGetChangeAsync(Guid id)
    {
        BackLinkPath = linkGenerator.ConfirmAccountDetailsUpdate(id);

        return await OnGetAsync(id);
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        Id = id;
        var accountDetails = new AccountDetails
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            SocialWorkEnglandNumber = SocialWorkEnglandNumber
        };
        var result = await validator.ValidateAsync(accountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath ??= linkGenerator.ViewAccountDetails(id);
            return Page();
        }

        await editAccountJourneyService.SetAccountDetailsAsync(id, accountDetails);

        return Redirect(linkGenerator.ConfirmAccountDetailsUpdate(id));
    }

    public async Task<IActionResult> OnPostChangeAsync(Guid id)
    {
        BackLinkPath = linkGenerator.ConfirmAccountDetailsUpdate(id);
        return await OnPostAsync(id);
    }
}
