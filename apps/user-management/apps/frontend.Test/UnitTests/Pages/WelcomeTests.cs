using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages;

public class WelcomeTests : PageModelTestBase<Welcome>
{
    public WelcomeTests()
    {
        Sut.PageContext.HttpContext = HttpContext;
    }

    private Welcome Sut { get; } = new(new FakeLinkGenerator());

    [Fact]
    public void OnGet_WhenCalledWithNoUser_RedirectsToIndexPage()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipal();

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        redirectResult.Url.Should().Be("index");
    }

    [Fact]
    public void OnGet_WhenCalledWithUser_LoadsPage()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithName(UserConstants.UserName)
            .WithEmail(UserConstants.UserEmail).WithRole(AccountType.EarlyCareerSocialWorker).Build();

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void OnGet_WhenCalledWithSocialWorkerUser_ShowsSocialWorkerContent()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithName(UserConstants.UserName)
            .WithEmail(UserConstants.UserEmail).WithRole(AccountType.EarlyCareerSocialWorker).Build();

        // Act
        Sut.OnGet();

        // Assert
        Sut.ShowSocialWorkerContent.Should().BeTrue();
    }

    [Theory]
    [InlineData(new[] { AccountType.Assessor })]
    [InlineData(new[] { AccountType.Coordinator })]
    [InlineData(new[] { AccountType.Assessor, AccountType.Coordinator })]
    public void OnGet_WhenCalledWithNonSocialWorkerUser_DoesNotShowSocialWorkerContent(AccountType[] accountTypes)
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithName(UserConstants.UserName)
            .WithEmail(UserConstants.UserEmail).WithRoles(accountTypes).Build();

        // Act
        Sut.OnGet();

        // Assert
        Sut.ShowSocialWorkerContent.Should().BeFalse();
    }
}
