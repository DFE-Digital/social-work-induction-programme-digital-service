using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
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
    [Display(Name = "Middle names")]
    public string? MiddlesNames { get; set; }

    [BindProperty]
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    [BindProperty]
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    [BindProperty]
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }

    public bool ShowSweInput { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (accountDetails is null)
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ManageAccount.ViewAccountDetails(id);
        Id = id;

        FirstName = accountDetails.FirstName;
        MiddlesNames = accountDetails.MiddleNames ?? string.Empty;
        LastName = accountDetails.LastName;
        Email = accountDetails.Email;

        var isSwe = SocialWorkEnglandRecord.TryParse(
            accountDetails.SocialWorkEnglandNumber,
            out var swe
        );
        SocialWorkEnglandNumber = isSwe ? swe?.GetNumber().ToString() : null;

        var accountTypes = accountDetails.Types;
        ShowSweInput = (accountTypes?.Contains(AccountType.EarlyCareerSocialWorker) ?? false) || (accountTypes?.Contains(AccountType.Assessor) ?? false);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (accountDetails is null)
        {
            return BadRequest();
        }

        accountDetails.FirstName = FirstName;
        accountDetails.MiddleNames = MiddlesNames;
        accountDetails.LastName = LastName;
        accountDetails.Email = Email;
        accountDetails.SocialWorkEnglandNumber = SocialWorkEnglandNumber;

        var accountTypes = accountDetails.Types;
        ShowSweInput = (accountTypes?.Contains(AccountType.EarlyCareerSocialWorker) ?? false) || (accountTypes?.Contains(AccountType.Assessor) ?? false);

        var result = await validator.ValidateAsync(accountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath ??= linkGenerator.ManageAccount.ViewAccountDetails(id);
            return Page();
        }

        await editAccountJourneyService.SetAccountDetailsAsync(id, accountDetails);

        return Redirect(linkGenerator.ManageAccount.ConfirmAccountDetailsUpdate(id));
    }
}
