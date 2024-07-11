using Dfe.Sww.Ecf.Frontend.Controllers;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.HomeControllerTests;

public abstract class HomeControllerTestBase
{
    private protected HomeController Sut { get; }

    protected HomeControllerTestBase()
    {
        Sut = new HomeController();
    }
}
