using Xunit;

namespace Dfe.Sww.Ecf.UiTestCommon;

public static class AssertEx
{
    public static void HtmlResponse(HttpResponseMessage response, int expectedStatusCode = 200)
    {
        Assert.Equal(expectedStatusCode, (int)response.StatusCode);
    }
}
