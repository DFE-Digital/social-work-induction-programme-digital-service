using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

[AuthorizeRoles(RoleType.Coordinator)]
public class ConfirmUserDetails(
    ICreateUserJourneyService createUserJourneyService,
    IEditUserJourneyService editUserJourneyService,
    IMoodleServiceClient moodleServiceClient,
    EcfLinkGenerator linkGenerator
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

    public bool IsUpdatingUser { get; set; }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.AddUserDetails();
        ChangeDetailsLink = linkGenerator.AddUserDetailsChange();

        var accountDetails = createUserJourneyService.GetUserDetails();

        FirstName = accountDetails?.FirstName;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;

        return Page();
    }

    public async Task<IActionResult> OnGetUpdateAsync(Guid id)
    {
        var updatedUserDetails = await editUserJourneyService.GetUserDetailsAsync(id);
        if (updatedUserDetails is null)
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.EditUserDetails(id);
        ChangeDetailsLink = linkGenerator.EditUserDetailsChange(id);

        IsUpdatingUser = true;
        Id = id;

        FirstName = updatedUserDetails.FirstName;
        LastName = updatedUserDetails.LastName;
        Email = updatedUserDetails.Email;
        SocialWorkEnglandNumber = updatedUserDetails.SocialWorkEnglandNumber;

        return Page();
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var accountDetails = createUserJourneyService.GetUserDetails();
        if (accountDetails is null)
        {
            return BadRequest();
        }

        var moodleRequest = new CreateMoodleUserRequest
        {
            Username = accountDetails.Email,
            Email = accountDetails.Email,
            FirstName = accountDetails.FirstName,
            LastName = accountDetails.LastName
        };
        var response = await moodleServiceClient.User.CreateUserAsync(moodleRequest);
        if (response.Successful == false)
        {
            return BadRequest();
        }

        createUserJourneyService.SetExternalUserId(response.Id);

        await createUserJourneyService.CompleteJourneyAsync();

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = "New user added";
        TempData["NotificationMessage"] = $"An invitation to register has been sent to {accountDetails.FullName}, {accountDetails.Email}";

        return Redirect(linkGenerator.ManageUsers());
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid id)
    {
        if (!await editUserJourneyService.IsUserIdValidAsync(id))
        {
            return NotFound();
        }

        await editUserJourneyService.CompleteJourneyAsync(id);

        return Redirect(linkGenerator.ViewUserDetails(id));
    }
}
