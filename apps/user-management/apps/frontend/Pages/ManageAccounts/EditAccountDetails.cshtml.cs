using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Routing;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EditAccountDetails(
    IAccountRepository accountRepository,
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
        var account = accountRepository.GetById(id);
        if (account is null)
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ViewAccountDetails(id);
        Id = id;

        var updatedAccountDetails = TempData.Peek<AccountDetails>("UpdatedAccountDetails-" + id);

        FirstName = updatedAccountDetails?.FirstName ?? account.FirstName;
        LastName = updatedAccountDetails?.LastName ?? account.LastName;
        Email = updatedAccountDetails?.Email ?? account.Email;
        SocialWorkEnglandNumber =
            updatedAccountDetails?.SocialWorkEnglandNumber ?? account.SocialWorkEnglandNumber;

        return Page();
    }

    public IActionResult OnGetChange(Guid id)
    {
        BackLinkPath = linkGenerator.ConfirmAccountDetailsUpdate(id);

        return OnGet(id);
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (accountRepository.GetById(id) is null)
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
            return Page();
        }

        TempData.Set("UpdatedAccountDetails-" + id, accountDetails);

        return Redirect(linkGenerator.ConfirmAccountDetailsUpdate(id));
    }
}
