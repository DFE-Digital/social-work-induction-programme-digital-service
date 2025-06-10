using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public abstract class SocialWorkerRegistrationPageTestBase<[MeansTestSubject] T> : PageModelTestBase<T>
    where T : PageModel
{
    protected SocialWorkerRegistrationPageTestBase()
    {
    }

    private protected void VerifyAllNoOtherCalls()
    {
    }
}
