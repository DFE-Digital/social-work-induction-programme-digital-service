using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

public class EnterLocalAuthorityCode(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    IAuthServiceClient authServiceClient,
    EcfLinkGenerator linkGenerator,
    IValidator<EnterLocalAuthorityCode> validator
) : BasePageModel
{
    [BindProperty] public string? LocalAuthorityCode { get; set; }

    public PageResult OnGet()
    {
        var localAuthorityCode = createOrganisationJourneyService.GetLocalAuthorityCode();
        if (localAuthorityCode is not null)
        {
            LocalAuthorityCode = localAuthorityCode.ToString();
        }

        SetBackLinkPath();
        return Page();
    }

    public RedirectResult OnGetNew()
    {
        createOrganisationJourneyService.ResetCreateOrganisationJourneyModel();
        return Redirect(linkGenerator.ManageOrganisations.EnterLocalAuthorityCode());
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return OnGet();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var initialCode = createOrganisationJourneyService.GetLocalAuthorityCode();

        var noChange = initialCode.HasValue
                       && string.Equals(LocalAuthorityCode?.Trim(), initialCode.Value.ToString(), StringComparison.Ordinal);
        var validationContext = new ValidationContext<EnterLocalAuthorityCode>(this)
        {
            RootContextData =
            {
                ["SkipLaCodeUnique"] = noChange
            }
        };

        var result = await validator.ValidateAsync(validationContext);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
            return Page();
        }

        if (!int.TryParse(LocalAuthorityCode, out var parsedLocalAuthorityCode))
        {
            ModelState.AddModelError(nameof(LocalAuthorityCode), "The local authority code must only include numbers");
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
            return Page();
        }

        createOrganisationJourneyService.SetLocalAuthorityCode(parsedLocalAuthorityCode);

        var organisation = await authServiceClient.LocalAuthority.GetByLocalAuthorityCodeAsync(parsedLocalAuthorityCode);
        if (organisation is null)
        {
            ModelState.AddModelError(nameof(LocalAuthorityCode), "The code you have entered is not associated with a local authority");
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
            return Page();
        }

        createOrganisationJourneyService.SetOrganisation(organisation);

        return Redirect(
            FromChangeLink
                ? linkGenerator.ManageOrganisations.CheckYourAnswers()
                : linkGenerator.ManageOrganisations.ConfirmOrganisationDetails()
        );
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return await OnPostAsync();
    }

    private void SetBackLinkPath()
    {
        BackLinkPath ??= FromChangeLink
            ? linkGenerator.ManageOrganisations.CheckYourAnswers()
            : linkGenerator.ManageOrganisations.Index();
    }
}
