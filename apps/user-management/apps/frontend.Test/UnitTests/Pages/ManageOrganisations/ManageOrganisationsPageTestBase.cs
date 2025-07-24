using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public abstract class ManageOrganisationsPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    private protected OrganisationBuilder OrganisationBuilder { get; }

    private protected AccountBuilder AccountBuilder { get; }

    private protected Mock<IOrganisationService> MockOrganisationService { get; }

    private protected Mock<ICreateOrganisationJourneyService> MockCreateOrganisationJourneyService { get; }
    private protected Mock<IEditOrganisationJourneyService> MockEditOrganisationJourneyService { get; }

    private protected Mock<IAccountService> MockAccountService { get; }

    protected ManageOrganisationsPageTestBase()
    {
        OrganisationBuilder = new();

        AccountBuilder = new();

        MockOrganisationService = new();

        MockCreateOrganisationJourneyService = new();

        MockEditOrganisationJourneyService = new();

        MockAccountService = new();
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockOrganisationService.VerifyNoOtherCalls();
        MockCreateOrganisationJourneyService.VerifyNoOtherCalls();
        MockEditOrganisationJourneyService.VerifyNoOtherCalls();
        MockAccountService.VerifyNoOtherCalls();
    }
}
