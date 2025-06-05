using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

[AuthorizeRoles(RoleType.Coordinator)]
public class EditUserDetails(
    IEditUserJourneyService editUserJourneyService,
    IValidator<UserDetails> validator,
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
        var accountDetails = await editUserJourneyService.GetUserDetailsAsync(id);
        if (accountDetails is null)
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ViewUserDetails(id);
        Id = id;

        FirstName = accountDetails.FirstName;
        LastName = accountDetails.LastName;
        Email = accountDetails.Email;

        var isSwe = SocialWorkEnglandRecord.TryParse(
            accountDetails.SocialWorkEnglandNumber,
            out var swe
        );
        SocialWorkEnglandNumber = isSwe ? swe?.GetNumber().ToString() : null;
        IsStaff = await editUserJourneyService.GetIsStaffAsync(id) ?? false;

        return Page();
    }

    public async Task<IActionResult> OnGetChangeAsync(Guid id)
    {
        BackLinkPath = linkGenerator.ConfirmUserDetailsUpdate(id);

        return await OnGetAsync(id);
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!await editUserJourneyService.IsUserIdValidAsync(id))
        {
            return NotFound();
        }

        Id = id;
        var accountDetails = new UserDetails
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            SocialWorkEnglandNumber = SocialWorkEnglandNumber,
            IsStaff = IsStaff
        };
        var result = await validator.ValidateAsync(accountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath ??= linkGenerator.ViewUserDetails(id);
            return Page();
        }

        await editUserJourneyService.SetUserDetailsAsync(id, accountDetails);

        return Redirect(linkGenerator.ConfirmUserDetailsUpdate(id));
    }

    public async Task<IActionResult> OnPostChangeAsync(Guid id)
    {
        BackLinkPath = linkGenerator.ConfirmUserDetailsUpdate(id);
        return await OnPostAsync(id);
    }
}
