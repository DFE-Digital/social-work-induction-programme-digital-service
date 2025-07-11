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

public class DashboardTests : PageModelTestBase<Dashboard>
{
    public DashboardTests()
    {
        Sut.PageContext.HttpContext = HttpContext;
    }

    private Dashboard Sut { get; } = new();

    [Fact]
    public void OnGet_WhenCalled_LoadsPage()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void OnGet_WhenCalledWithItemForManageOrganisations_IncludesItem()
    {
        // Act
        Sut.OnGet();

        // Assert
        Assert.NotNull(Sut.Items);
        var items = Sut.Items!.ToList();
        Assert.Single(items);

        var card = items[0];
        Assert.Equal("Manage organisations", card.Link?.Text);
        Assert.Equal("/manage-organisations", card.Link?.Path);
        Assert.Equal("Add or edit organisations and manage users.", card.Description);
    }
}
