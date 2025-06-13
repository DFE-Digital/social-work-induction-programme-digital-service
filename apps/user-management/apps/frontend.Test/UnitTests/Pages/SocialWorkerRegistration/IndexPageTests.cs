using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;
using SocialWorkerRegistrationIndex = Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class IndexPageTests : SocialWorkerRegistrationPageTestBase
{
    private SocialWorkerRegistrationIndex Sut { get; }

    public IndexPageTests()
    {
        Sut = new();
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsPage()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }
}
