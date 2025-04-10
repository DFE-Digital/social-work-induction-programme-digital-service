using System.ComponentModel.DataAnnotations;
using SocialWorkInductionProgramme.Frontend.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialWorkInductionProgramme.Frontend.Authorisation;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;
using SocialWorkInductionProgramme.Frontend.Routing;
using SocialWorkInductionProgramme.Frontend.Services.Journeys.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Pages.ManageAccounts;

/// <summary>
/// Add User Details View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class AddAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IValidator<AccountDetails> validator,
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

    public bool IsStaff = createAccountJourneyService.GetIsStaff() ?? false;

    private void SetBackLinkPath(bool fromConfirmPage = false)
    {
        BackLinkPath ??= fromConfirmPage
            ? linkGenerator.ConfirmAccountDetails()
            : IsStaff
                ? linkGenerator.SelectUseCase()
                : linkGenerator.SelectAccountType();
    }

    public PageResult OnGet()
    {
        SetBackLinkPath();

        var accountDetails = createAccountJourneyService.GetAccountDetails();

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
            SetBackLinkPath();
            return Page();
        }

        createAccountJourneyService.SetAccountDetails(accountDetails);

        return Redirect(linkGenerator.ConfirmAccountDetails());
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        SetBackLinkPath(true);
        return await OnPostAsync();
    }
}
