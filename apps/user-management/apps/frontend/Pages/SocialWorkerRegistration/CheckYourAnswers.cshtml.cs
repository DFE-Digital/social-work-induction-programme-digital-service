using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class CheckYourAnswers(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService registerSocialWorkerJourneyService,
    IAuthServiceClient authServiceClient
) : BasePageModel
{
    public string ChangeDetailsLink { get; set; } = linkGenerator.SocialWorkerRegistration(); // TODO update this to relevant change link per row

    [Display(Name = "Date of birth")] public string? DateOfBirth { get; set; }

    [Display(Name = "What is your sex?")] public UserSex? UserSex { get; set; }

    [Display(Name = "Is the gender you identify with the same as your sex registered at birth?")]
    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; }

    [Display(Name = "What is your gender identity?")]
    public string? OtherGenderIdentity { get; set; }

    [Display(Name = "What is your ethnic group?")]
    public EthnicGroup? EthnicGroup { get; set; }

    [Display(Name = "How would you describe your background?")]
    public string? OtherEthnicGroup { get; set; }

    [Display(Name = "Do you have a disability?")]
    public Disability? Disability { get; set; }

    [Display(Name = "What date were you added to the Social Work England register?")]
    public string? SocialWorkEnglandRegistrationDate { get; set; }

    [Display(Name = "What is the highest qualification you hold?")]
    public Qualification? HighestQualification { get; set; }

    [Display(Name = "What year did you finish your social work qualification?")]
    public int? SocialWorkQualificationEndYear { get; set; }

    [Display(Name = "What entry route into social work did you take?")]
    public RouteIntoSocialWork? RouteIntoSocialWork { get; set; }

    [Display(Name = "What entry route into social work did you take?")]
    public string? OtherRouteIntoSocialWork { get; set; }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public async Task<PageResult> OnGetAsync()
    {
        BackLinkPath = linkGenerator.SocialWorkerRegistrationSelectRouteIntoSocialWork();

        var personId = authServiceClient.HttpContextService.GetPersonId();

        var registerModel = await registerSocialWorkerJourneyService.GetRegisterSocialWorkerJourneyModelAsync(personId);

        DateOfBirth = registerModel?.DateOfBirth?.ToString("d MMMM yyyy");
        UserSex = registerModel?.UserSex;
        GenderMatchesSexAtBirth = registerModel?.GenderMatchesSexAtBirth;
        EthnicGroup = registerModel?.EthnicGroup;
        OtherEthnicGroup = GetOtherEthnicGroup(registerModel);
        Disability = registerModel?.Disability;
        SocialWorkEnglandRegistrationDate = registerModel?.SocialWorkEnglandRegistrationDate?.ToString("d MMMM yyyy");
        HighestQualification = registerModel?.HighestQualification;
        SocialWorkQualificationEndYear = registerModel?.SocialWorkQualificationEndYear;
        RouteIntoSocialWork = registerModel?.RouteIntoSocialWork;
        OtherRouteIntoSocialWork = registerModel?.OtherRouteIntoSocialWork;
        OtherGenderIdentity = registerModel?.OtherGenderIdentity;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();

        await registerSocialWorkerJourneyService.CompleteJourneyAsync(personId);

        return Redirect(linkGenerator.ManageAccounts()); // TODO update this to the registration complete page
    }

    private static string? GetOtherEthnicGroup(RegisterSocialWorkerJourneyModel? accountDetails)
    {
        return accountDetails?.OtherEthnicGroupWhite
               ?? accountDetails?.OtherEthnicGroupMixed
               ?? accountDetails?.OtherEthnicGroupAsian
               ?? accountDetails?.OtherEthnicGroupBlack
               ?? accountDetails?.OtherEthnicGroupOther;
    }
}
