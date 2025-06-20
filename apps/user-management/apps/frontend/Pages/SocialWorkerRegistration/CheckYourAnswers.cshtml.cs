using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Users;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class CheckYourAnswers(
    IRegisterSocialWorkerJourneyService registerSocialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    public string ChangeDetailsLink { get; set; } = linkGenerator.ManageAccounts();

    [Display(Name = "Date of birth")] public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "What is your sex?")] public UserSex? UserSex { get; set; }

    [Display(Name = "Is the gender you identify with the same as your sex registered at birth?")]
    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; }

    [Display(Name = "What is your ethnic group?")]
    public EthnicGroup? EthnicGroup { get; set; }

    [Display(Name = "Do you have a disability?")]
    public Disability? Disability { get; set; }

    [Display(Name = "What date were you added to the Social Work England register?")]
    public DateOnly? SocialWorkEnglandRegistrationDate { get; set; }

    [Display(Name = "What is the highest qualification you hold?")]
    public Qualification? HighestQualification { get; set; }

    [Display(Name = "What year did you finish your social work qualification?")]
    public int? SocialWorkQualificationEndYear { get; set; }

    // [Display(Name = "What entry route into social work did you take?")]
    // public RouteIntoSocialWork? RouteIntoSocialWork { get; set; }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public async Task<PageResult> OnGetAsync()
    {
        BackLinkPath = linkGenerator.SocialWorkerProgrammeDates();
        // ChangeDetailsLink = linkGenerator.AddAccountDetailsChange();

        var personId = authServiceClient.HttpContextService.GetPersonId();

        var accountDetails = await registerSocialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(personId);

        DateOfBirth = accountDetails?.DateOfBirth;
        UserSex = accountDetails?.UserSex;
        GenderMatchesSexAtBirth = accountDetails?.GenderMatchesSexAtBirth;
        EthnicGroup = accountDetails?.EthnicGroup;
        Disability = accountDetails?.Disability;
        SocialWorkEnglandRegistrationDate = accountDetails?.SocialWorkEnglandRegistrationDate;
        HighestQualification = accountDetails?.HighestQualification;
        SocialWorkQualificationEndYear = accountDetails?.SocialWorkQualificationEndYear;

        // RegisteredWithSocialWorkEngland = accountLabels?.IsRegisteredWithSocialWorkEnglandLabel;
        // StatutoryWorker = accountLabels?.IsStatutoryWorkerLabel;
        // AgencyWorker = accountLabels?.IsAgencyWorkerLabel;
        // Qualified = accountLabels?.IsQualifiedWithin3Years;
        // FirstName = accountDetails?.FirstName;
        // MiddleNames = accountDetails?.MiddleNames;
        // LastName = accountDetails?.LastName;
        // Email = accountDetails?.Email;
        // SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;
        // ProgrammeStartDate = createAccountJourneyService.GetProgrammeStartDate()?
        //     .ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        // ProgrammeEndDate = createAccountJourneyService.GetProgrammeEndDate()?
        //     .ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        return Page();
    }

    // /// <summary>
    // /// Action for confirming user details
    // /// </summary>
    // /// <returns>A confirmation screen displaying user details</returns>
    // public async Task<IActionResult> OnPostAsync()
    // {
    //     var accountDetails = createAccountJourneyService.GetAccountDetails();
    //     if (accountDetails is null)
    //     {
    //         return BadRequest();
    //     }
    //
    //     var moodleRequest = new CreateMoodleUserRequest
    //     {
    //         Username = accountDetails.Email,
    //         Email = accountDetails.Email,
    //         FirstName = accountDetails.FirstName,
    //         LastName = accountDetails.LastName
    //     };
    //     var response = await moodleServiceClient.User.CreateUserAsync(moodleRequest);
    //     if (response.Successful == false)
    //     {
    //         return BadRequest();
    //     }
    //
    //     createAccountJourneyService.SetExternalUserId(response.Id);
    //
    //     await createAccountJourneyService.CompleteJourneyAsync();
    //
    //     TempData["NotificationType"] = NotificationBannerType.Success;
    //     TempData["NotificationHeader"] = "New user added";
    //     TempData["NotificationMessage"] = $"An invitation to register has been sent to {accountDetails.FullName}, {accountDetails.Email}";
    //
    //     return Redirect(linkGenerator.ManageAccounts());
    // }

    // public async Task<IActionResult> OnPostUpdateAsync(Guid id)
    // {
    //     if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
    //     {
    //         return NotFound();
    //     }
    //
    //     await editAccountJourneyService.CompleteJourneyAsync(id);
    //
    //     return Redirect(linkGenerator.ViewAccountDetails(id));
    // }
}
