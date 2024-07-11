using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.HomeControllerTests;

public class IndexShould : HomeControllerTestBase
{
    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }
}
