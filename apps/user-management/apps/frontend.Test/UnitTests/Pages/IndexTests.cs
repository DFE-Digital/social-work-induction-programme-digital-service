using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;
using Index = Dfe.Sww.Ecf.Frontend.Pages.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages;

public class IndexTests : PageModelTestBase<Index>
{
    public IndexTests()
    {
        Sut.PageContext.HttpContext = HttpContext;
    }

    private Index Sut { get; } = new(new FakeLinkGenerator());

    [Fact]
    public void OnGet_WhenCalledWithNoUser_LoadsPage()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipal();

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void OnGet_WhenCalledWithUser_RedirectsToWelcomePage()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithName(UserConstants.UserName)
            .WithEmail(UserConstants.UserEmail).WithRole(RoleType.EarlyCareerSocialWorker).Build();

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        redirectResult.Url.Should().Be("/welcome");
    }
}
