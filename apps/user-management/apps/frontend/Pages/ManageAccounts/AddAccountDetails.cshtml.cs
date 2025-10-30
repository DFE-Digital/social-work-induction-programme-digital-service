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

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class AddAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IValidator<AccountDetails> validator,
    EcfLinkGenerator linkGenerator
) : ManageAccountsBasePageModel
{
    /// <summary>
    /// First Name
    /// </summary>
    [BindProperty]
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Middle Names
    /// </summary>
    [BindProperty]
    [Display(Name = "Middle names")]
    public string? MiddleNames { get; set; }

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

    public IList<AccountType> AccountTypes { get; set; } = createAccountJourneyService.GetAccountTypes() ?? new List<AccountType>();

    private void SetBackLinkPath(bool fromConfirmPage = false, bool isFunded = false)
    {
        BackLinkPath ??= fromConfirmPage
            ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId)
            : IsStaff
                ? linkGenerator.ManageAccount.SelectUseCase(OrganisationId)
                : isFunded
                    ? linkGenerator.ManageAccount.EligibilityFundingAvailable(OrganisationId)
                    : linkGenerator.ManageAccount.EligibilityFundingNotAvailable();
    }

    public PageResult OnGet()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        var isFunded = createAccountJourneyService.GetIsFunded();

        SetBackLinkPath(isFunded: isFunded ?? false);

        FirstName = accountDetails?.FirstName;
        MiddleNames = accountDetails?.MiddleNames;
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
            MiddleNames = MiddleNames,
            Email = Email,
            SocialWorkEnglandNumber = SocialWorkEnglandNumber,
            IsStaff = IsStaff,
            Types = AccountTypes
        };
        var result = await validator.ValidateAsync(accountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            var isFunded = createAccountJourneyService.GetIsFunded();
            SetBackLinkPath(isFunded: isFunded ?? false);
            return Page();
        }

        createAccountJourneyService.SetAccountDetails(accountDetails);

        return Redirect((IsStaff || FromChangeLink)
            ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId)
            : linkGenerator.ManageAccount.SocialWorkerProgrammeDates(OrganisationId));
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        SetBackLinkPath(true);
        return await OnPostAsync();
    }
}
