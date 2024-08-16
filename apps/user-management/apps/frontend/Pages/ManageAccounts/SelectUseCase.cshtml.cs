using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class SelectUseCase(
    ICreateAccountJourneyService createAccountJourneyService,
    IValidator<SelectUseCase> validator,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public IList<AccountType>? SelectedAccountTypes { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.SelectAccountType();
        SelectedAccountTypes = createAccountJourneyService.GetAccountTypes();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (SelectedAccountTypes is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return Page();
        }

        createAccountJourneyService.SetAccountTypes(SelectedAccountTypes);

        return Redirect(linkGenerator.AddAccountDetails());
    }
}
