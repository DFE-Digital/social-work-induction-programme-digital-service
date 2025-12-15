using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Error;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using ManageAccountsIndex = Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.Error;

public class IndexPageTests
{
    private ErrorModel Sut { get; }

    public IndexPageTests()
    {
        Sut = new ErrorModel();
    }

    [Theory]
    [InlineData(403, "NotAuthorised", "You do not have permission to view this page")]
    [InlineData(404, "PageNotFound", "Page not found")]
    [InlineData(500, "ProblemWithTheService", "Sorry, there is a problem with the service")]
    public void Get_WhenCalled_LoadsTheCorrectData(int statusCode, string partialName, string title)
    {
        // Act
        Sut.OnGet(statusCode);

        // Assert
        Sut.PartialName.Should().Be(partialName);
        Sut.Title.Should().Be(title);
    }
}
