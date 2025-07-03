using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
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
        var captureSocialWorkEnglandNumber = false;
        if (SelectedAccountTypes is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.SelectAccountType();
            return Page();
        }

        if (FromChangeLink)
        {
            SocialWorkEnglandNumberClearOrMarkForCapture(out captureSocialWorkEnglandNumber);
        }

        createAccountJourneyService.SetAccountTypes(SelectedAccountTypes);
        return Redirect(FromChangeLink && !captureSocialWorkEnglandNumber ? linkGenerator.ConfirmAccountDetails() : linkGenerator.AddAccountDetails());
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        return await OnPostAsync();
    }

    private void SocialWorkEnglandNumberClearOrMarkForCapture(out bool captureSocialWorkEnglandNumber)
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        captureSocialWorkEnglandNumber = false;
        if (SelectedAccountTypes?.Count == 1 && SelectedAccountTypes[0] == AccountType.Coordinator)
        {
            if (accountDetails is not null && accountDetails.SocialWorkEnglandNumber is not null)
            {
                var updatedAccountDetails = new AccountDetails
                {
                    FirstName = accountDetails.FirstName,
                    LastName = accountDetails.LastName,
                    MiddleNames = accountDetails.MiddleNames,
                    Email = accountDetails.Email,
                    SocialWorkEnglandNumber = null,
                    IsStaff = accountDetails.IsStaff,
                    Types = accountDetails.Types
                };
                createAccountJourneyService.SetAccountDetails(updatedAccountDetails);
            }
        } else if (SelectedAccountTypes?.Contains(AccountType.Assessor) == true)
        {
            if (accountDetails is not null && accountDetails.SocialWorkEnglandNumber is null)
            {
                captureSocialWorkEnglandNumber = true;
            }
        }
    }
}
