using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Add User Details View Model
/// </summary>
public class AddAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IValidator<AccountDetails> validator,
    EcfLinkGenerator linkGenerator,
    ISocialWorkEnglandService socialWorkEnglandService
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

        var socialWorker = await socialWorkEnglandService.GetById(SocialWorkEnglandNumber);

        string redirectLink;
        if (socialWorker is null)
        {
            redirectLink = linkGenerator.ConfirmAccountDetails();
        }
        else
        {
            createAccountJourneyService.SetSocialWorkerDetails(socialWorker);
            redirectLink = linkGenerator.AddExistingUser();
        }

        createAccountJourneyService.SetAccountDetails(accountDetails);

        return Redirect(redirectLink);
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        SetBackLinkPath(true);
        return await OnPostAsync();
    }
}
