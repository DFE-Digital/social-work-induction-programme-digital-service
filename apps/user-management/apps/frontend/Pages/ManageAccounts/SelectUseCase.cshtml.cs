using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class SelectUseCase(
    ICreateAccountJourneyService createAccountJourneyService,
    IValidator<SelectUseCase> validator,
    EcfLinkGenerator linkGenerator,
    IEditAccountJourneyService editAccountJourneyService
) : BasePageModel
{
    [BindProperty]
    public IList<AccountType>? SelectedAccountTypes { get; set; }

    public Guid? EditAccountId { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.SelectAccountType();
        SelectedAccountTypes = createAccountJourneyService.GetAccountTypes();
        return Page();
    }

    public async Task<IActionResult> OnGetEditAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.EditAccountType(id);
        EditAccountId = id;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (SelectedAccountTypes is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.SelectAccountType();
            return Page();
        }

        createAccountJourneyService.SetAccountTypes(SelectedAccountTypes);

        return Redirect(linkGenerator.AddAccountDetails());
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        var validationResult = await validator.ValidateAsync(this);
        if (SelectedAccountTypes is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.EditAccountType(id);
            EditAccountId = id;
            return Page();
        }

        await editAccountJourneyService.SetAccountTypesAsync(id, SelectedAccountTypes);
        await editAccountJourneyService.CompleteJourneyAsync(id);

        return Redirect(linkGenerator.ViewAccountDetails(id));
    }
}
