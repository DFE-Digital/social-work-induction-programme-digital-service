using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages;

public class PageModelTestBase<[MeansTestSubject] T>
    where T : PageModel
{
    private protected HttpContext HttpContext { get; }

    private protected ITempDataDictionary TempData { get; }

    protected PageModelTestBase()
    {
        HttpContext = new DefaultHttpContext
        {
            Request = { Headers = { Referer = "test-referer" } },
            Session = new MockHttpSession()
        };

        TempData = new TempDataDictionary(HttpContext, Mock.Of<ITempDataProvider>());
    }
}
