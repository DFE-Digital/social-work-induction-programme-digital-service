using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
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

    public IActionResult OnGet(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ViewAccountDetails(id);
        Id = id;

        var accountDetails = editAccountJourneyService.GetAccountDetails(id);

        FirstName = accountDetails.FirstName;
        LastName = accountDetails.LastName;
        Email = accountDetails.Email;
        SocialWorkEnglandNumber = accountDetails.SocialWorkEnglandNumber;

        return Page();
    }

    public IActionResult OnGetChange(Guid id)
    {
        BackLinkPath = linkGenerator.ConfirmAccountDetailsUpdate(id);

        return OnGet(id);
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
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

        editAccountJourneyService.SetAccountDetails(id, accountDetails);

        return Redirect(linkGenerator.ConfirmAccountDetailsUpdate(id));
    }

    public async Task<IActionResult> OnPostChangeAsync(Guid id)
    {
        BackLinkPath = linkGenerator.ConfirmAccountDetailsUpdate(id);
        return await OnPostAsync(id);
    }
}
