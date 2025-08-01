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
    IEditAccountJourneyService editAccountJourneyService,
    IValidator<SelectUseCase> validator,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty] public IList<AccountType>? SelectedAccountTypes { get; set; }
    [BindProperty] public Guid? Id { get; set; }

    public async Task<PageResult> OnGetAsync(Guid? id = null)
    {
        BackLinkPath = linkGenerator.ManageAccount.SelectAccountType();

        if (id.HasValue)
        {
            BackLinkPath = linkGenerator.ManageAccount.ViewAccountDetails(id.Value);
            Id = id.Value;
            var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id.Value);
            SelectedAccountTypes = accountDetails?.Types;
            return Page();
        }

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
            BackLinkPath = linkGenerator.ManageAccount.SelectAccountType();
            return Page();
        }

        if (FromChangeLink)
        {
            captureSocialWorkEnglandNumber = await ClearOrMarkForCaptureSocialWorkEnglandNumberAsync(Id);
        }

        if (Id.HasValue)
        {
            return await OnPostUpdateAsync(Id.Value, captureSocialWorkEnglandNumber);
        }

        createAccountJourneyService.SetAccountTypes(SelectedAccountTypes);
        return Redirect(FromChangeLink && !captureSocialWorkEnglandNumber ? linkGenerator.ManageAccount.ConfirmAccountDetails() : linkGenerator.ManageAccount.AddAccountDetails());
    }

    private async Task<IActionResult> OnPostUpdateAsync(Guid id, bool captureSocialWorkEnglandNumber)
    {
        if (Id.HasValue == false || SelectedAccountTypes == null)
        {
            return Page();
        }

        await editAccountJourneyService.SetAccountTypesAsync(Id.Value, SelectedAccountTypes);

        return Redirect(captureSocialWorkEnglandNumber ? linkGenerator.ManageAccount.EditAccountDetails(Id.Value) : linkGenerator.ManageAccount.ConfirmAccountDetailsUpdate(Id.Value));
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        return await OnPostAsync();
    }

    private async Task<bool> ClearOrMarkForCaptureSocialWorkEnglandNumberAsync(Guid? id)
    {
        var accountDetails = id is null
            ? createAccountJourneyService.GetAccountDetails()
            : await editAccountJourneyService.GetAccountDetailsAsync(id.Value);

        if (SelectedAccountTypes is [AccountType.Coordinator] && accountDetails?.SocialWorkEnglandNumber is not null)
        {
            accountDetails.SocialWorkEnglandNumber = null;

            if (id is null)
            {
                createAccountJourneyService.SetAccountDetails(accountDetails);
            }
            else
            {
                await editAccountJourneyService.SetAccountDetailsAsync(id.Value, accountDetails);
            }

            return false;
        }

        return SelectedAccountTypes?.Contains(AccountType.Assessor) == true
               && accountDetails?.SocialWorkEnglandNumber is null;
    }
}
