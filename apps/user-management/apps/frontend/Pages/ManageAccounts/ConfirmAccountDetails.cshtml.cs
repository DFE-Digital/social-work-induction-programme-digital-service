using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ConfirmAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IEditAccountJourneyService editAccountJourneyService,
    IMoodleService moodleService,
    EcfLinkGenerator linkGenerator,
    IOptions<FeatureFlags> featureFlags
) : ManageAccountsBasePageModel
{
    public Guid Id { get; set; }

    [Display(Name = "Who do you want to add?")]
    public string? UserType { get; set; }

    [Display(Name = "Account type")] public IList<AccountType>? AccountTypes { get; set; }

    [Display(Name = "Are they working in statutory child and family social work?")]
    public string? StatutoryWorker { get; set; }

    [Display(Name = "Are they an agency worker?")]
    public string? AgencyWorker { get; set; }

    [Display(Name = "Have they completed their social work qualification within the last 3 years?")]
    public string? Qualified { get; set; }

    /// <summary>
    ///     First Name
    /// </summary>
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    [Display(Name = "Middle names")] public string? MiddleNames { get; set; }

    /// <summary>
    ///     Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    /// <summary>
    ///     Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    /// <summary>
    ///     Social Work England number
    /// </summary>
    [Display(Name = "Social Work England registration number")]
    public string? SocialWorkEnglandNumber { get; set; }

    [Display(Name = "Programme start date")]
    public string? ProgrammeStartDate { get; set; }

    [Display(Name = "Expected programme end date")]
    public string? ProgrammeEndDate { get; set; }

    public AccountChangeLinks ChangeDetailsLinks { get; set; } = new();

    public bool IsUpdatingAccount { get; set; }

    public bool? IsStaff { get; set; }
    public bool IsSocialWorker { get; set; }

    /// <summary>
    ///     Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public PageResult OnGet()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        var accountLabels = createAccountJourneyService.GetAccountLabels();
        var accountTypes = createAccountJourneyService.GetAccountTypes();

        BackLinkPath = linkGenerator.ManageAccount.SocialWorkerProgrammeDates(OrganisationId);
        ChangeDetailsLinks = createAccountJourneyService.GetAccountChangeLinks(accountTypes?.Contains(AccountType.EarlyCareerSocialWorker) ?? false, OrganisationId);

        UserType = accountLabels?.IsStaffLabel;
        AccountTypes = accountTypes;
        StatutoryWorker = accountLabels?.IsStatutoryWorkerLabel;
        AgencyWorker = accountLabels?.IsAgencyWorkerLabel;
        Qualified = accountLabels?.IsRecentlyQualifiedLabel;
        FirstName = accountDetails?.FirstName;
        MiddleNames = accountDetails?.MiddleNames;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;
        ProgrammeStartDate = createAccountJourneyService.GetProgrammeStartDate()?
            .ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        ProgrammeEndDate = createAccountJourneyService.GetProgrammeEndDate()?
            .ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        IsStaff = accountDetails?.IsStaff;
        IsSocialWorker = AccountTypes != null && (AccountTypes.Contains(AccountType.Assessor) || AccountTypes.Contains(AccountType.EarlyCareerSocialWorker));

        return Page();
    }

    public async Task<IActionResult> OnGetUpdateAsync(Guid id)
    {
        var updatedAccountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (updatedAccountDetails is null) return NotFound();

        BackLinkPath = linkGenerator.ManageAccount.EditAccountDetails(id, OrganisationId);
        ChangeDetailsLinks = editAccountJourneyService.GetAccountChangeLinks(id, OrganisationId);

        IsUpdatingAccount = true;
        Id = id;

        IsStaff = updatedAccountDetails.IsStaff;
        AccountTypes = updatedAccountDetails.Types;
        FirstName = updatedAccountDetails.FirstName;
        MiddleNames = updatedAccountDetails.MiddleNames;
        LastName = updatedAccountDetails.LastName;
        Email = updatedAccountDetails.Email;
        SocialWorkEnglandNumber = updatedAccountDetails.SocialWorkEnglandNumber;
        ProgrammeStartDate = updatedAccountDetails.ProgrammeStartDate?
            .ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        ProgrammeEndDate = updatedAccountDetails.ProgrammeEndDate?
            .ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        return Page();
    }

    /// <summary>
    ///     Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        if (accountDetails is null) return BadRequest();

        if (featureFlags.Value.EnableMoodleIntegration)
        {
            var externalUserId = await moodleService.CreateUserAsync(accountDetails);
            if (externalUserId is null) return BadRequest(); // TODO handle unhappy path in separate ticket

            createAccountJourneyService.SetExternalUserId(externalUserId);
        }

        await createAccountJourneyService.CompleteJourneyAsync(OrganisationId);

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = "New user added";
        TempData["NotificationMessage"] = $"An invitation to register has been sent to {accountDetails.FullName}, {accountDetails.Email}";

        return Redirect(linkGenerator.ManageAccount.Index(OrganisationId));
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id)) return NotFound();

        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (accountDetails is null) return BadRequest();

        if (featureFlags.Value.EnableMoodleIntegration)
        {
            var externalUserId = await moodleService.UpdateUserAsync(accountDetails);
            if (externalUserId is null) return BadRequest(); // TODO handle unhappy path in separate ticket

            createAccountJourneyService.SetExternalUserId(externalUserId);
        }

        await editAccountJourneyService.CompleteJourneyAsync(id);

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = "User details updated";
        TempData["NotificationMessage"] = $"An email has been sent to {accountDetails.FullName}, {accountDetails.Email}";

        return Redirect(linkGenerator.ManageAccount.ViewAccountDetails(id, OrganisationId));
    }
}
