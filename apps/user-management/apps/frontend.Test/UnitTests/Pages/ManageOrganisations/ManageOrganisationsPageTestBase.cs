using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageOrganisations;

public abstract class ManageOrganisationsPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    private protected OrganisationBuilder OrganisationBuilder { get; }

    private protected Mock<IOrganisationService> MockOrganisationService { get; }

    protected ManageOrganisationsPageTestBase()
    {
        OrganisationBuilder = new();

        MockOrganisationService = new();
    }

    private protected void VerifyAllNoOtherCalls()
    {
        MockOrganisationService.VerifyNoOtherCalls();
    }
}
