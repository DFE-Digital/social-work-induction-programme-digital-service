using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

/// <summary>
/// Add User Details View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class AddUserDetails(
    ICreateUserJourneyService createUserJourneyService,
    IValidator<UserDetails> validator,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    /// <summary>
    /// First Name
    /// </summary>
    [BindProperty]
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last Name
    /// </summary>
    [BindProperty]
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [BindProperty]
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [BindProperty]
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }

    public bool IsStaff = createUserJourneyService.GetIsStaff() ?? false;

    private void SetBackLinkPath(bool fromConfirmPage = false)
    {
        BackLinkPath ??= fromConfirmPage
            ? linkGenerator.ConfirmUserDetails()
            : IsStaff
                ? linkGenerator.SelectUseCase()
                : linkGenerator.SelectUserType();
    }

    public PageResult OnGet()
    {
        SetBackLinkPath();

        var accountDetails = createUserJourneyService.GetUserDetails();

        FirstName = accountDetails?.FirstName;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;

        return Page();
    }

    public PageResult OnGetChange()
    {
        SetBackLinkPath(true);
        return OnGet();
    }

    public async Task<IActionResult> OnPostAsync()
    {
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
            SetBackLinkPath();
            return Page();
        }

        createUserJourneyService.SetUserDetails(accountDetails);

        return Redirect(linkGenerator.SocialWorkerProgrammeDates());
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        SetBackLinkPath(true);
        return await OnPostAsync();
    }
}
