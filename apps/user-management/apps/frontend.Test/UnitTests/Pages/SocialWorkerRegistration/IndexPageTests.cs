using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;
using SocialWorkerRegistrationIndex = Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class IndexPageTests : SocialWorkerRegistrationPageTestBase<SocialWorkerRegistrationIndex>
{
    private SocialWorkerRegistrationIndex Sut { get; }

    public IndexPageTests()
    {
        Sut = new(new FakeLinkGenerator());
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsPage()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void OnPost_WhenCalled_RedirectsToHome()
    {
        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("index"); // TODO update this ECSW DoB page
    }
}
