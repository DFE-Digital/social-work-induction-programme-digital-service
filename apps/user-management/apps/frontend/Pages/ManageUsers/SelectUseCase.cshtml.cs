using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

[AuthorizeRoles(RoleType.Coordinator)]
public class SelectUseCase(
    ICreateUserJourneyService createUserJourneyService,
    IValidator<SelectUseCase> validator,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public IList<UserType>? SelectedUserTypes { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.SelectUserType();
        SelectedUserTypes = createUserJourneyService.GetUserTypes();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (SelectedUserTypes is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.SelectUserType();
            return Page();
        }

        createUserJourneyService.SetUserTypes(SelectedUserTypes);

        return Redirect(linkGenerator.AddUserDetails());
    }
}
