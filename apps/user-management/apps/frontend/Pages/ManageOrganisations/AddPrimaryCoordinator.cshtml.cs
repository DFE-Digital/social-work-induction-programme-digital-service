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

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

/// <summary>
/// Add Primary Coordinator View Model
/// </summary>
[AuthorizeRoles(RoleType.Administrator)]
public class AddPrimaryCoordinator(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
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
    /// Phone number
    /// </summary>
    [BindProperty]
    [Display(Name = "UK phone number")]
    public string? PhoneNumber { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ConfirmOrganisationDetails();
        var accountDetails = createOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails();

        FirstName = accountDetails?.FirstName;
        MiddleNames = accountDetails?.MiddleNames;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        PhoneNumber = accountDetails?.PhoneNumber;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var accountDetails = new AccountDetails
        {
            FirstName = FirstName,
            LastName = LastName,
            MiddleNames = MiddleNames,
            Email = Email,
            PhoneNumber = PhoneNumber,
            PhoneNumberRequired = true
        };
        var result = await validator.ValidateAsync(accountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.EnterLocalAuthorityCode();
            return Page();
        }

        createOrganisationJourneyService.SetPrimaryCoordinatorAccountDetails(accountDetails);

        // TODO replace with confirm details including coordinator if using a different page to confirm details for organisation only
        return Redirect(linkGenerator.ConfirmOrganisationDetails());
    }


}
