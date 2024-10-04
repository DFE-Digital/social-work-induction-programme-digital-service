using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ConfirmAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IEditAccountJourneyService editAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    INotificationServiceClient notificationServiceClient,
    IOptions<EmailTemplateOptions> emailTemplateOptions
) : BasePageModel
{
    public Guid Id { get; set; }

    /// <summary>
    /// First Name
    /// </summary>
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }

    public string? ChangeDetailsLink { get; set; }

    public bool IsUpdatingAccount { get; set; }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.AddAccountDetails();
        ChangeDetailsLink = linkGenerator.AddAccountDetailsChange();

        var accountDetails = createAccountJourneyService.GetAccountDetails();

        FirstName = accountDetails?.FirstName;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;

        return Page();
    }

    public IActionResult OnGetUpdate(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.EditAccountDetails(id);
        ChangeDetailsLink = linkGenerator.EditAccountDetailsChange(id);

        IsUpdatingAccount = true;
        Id = id;

        var updatedAccountDetails = editAccountJourneyService.GetAccountDetails(id);

        FirstName = updatedAccountDetails.FirstName;
        LastName = updatedAccountDetails.LastName;
        Email = updatedAccountDetails.Email;
        SocialWorkEnglandNumber = updatedAccountDetails.SocialWorkEnglandNumber;

        return Page();
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public async Task<RedirectResult> OnPost()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        await SendInvitationEmail(accountDetails);
        createAccountJourneyService.CompleteJourney();

        TempData["NotifyEmail"] = accountDetails?.Email;
        TempData["NotificationBannerSubject"] = "Account was successfully added";

        return Redirect(linkGenerator.ManageAccounts());
    }

    public IActionResult OnPostUpdate(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        editAccountJourneyService.CompleteJourney(id);

        return Redirect(linkGenerator.ViewAccountDetails(id));
    }

    private async Task SendInvitationEmail(AccountDetails? accountDetails)
    {
        var accountTypes = createAccountJourneyService.GetAccountTypes();

        if (accountTypes is null || accountDetails is null || string.IsNullOrWhiteSpace(accountDetails.Email))
        {
            return;
        }

        // Get the highest ranking role - the lowest (int)enum
        var invitationEmailType = accountTypes.Min();

        var templateId = emailTemplateOptions.Value.Roles[invitationEmailType.ToString()].Invitation;
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = accountDetails.Email,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", accountDetails.FullName },
                { "organisation", "TEST ORGANISATION" } // TODO Retrieve this value when we can
            }
        };

        await notificationServiceClient.Notification.SendEmailAsync(notificationRequest);
    }
}
