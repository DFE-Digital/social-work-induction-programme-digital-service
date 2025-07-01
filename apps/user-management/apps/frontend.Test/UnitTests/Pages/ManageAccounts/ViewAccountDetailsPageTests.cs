using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class ViewAccountDetailsPageTests : ManageAccountsPageTestBase<ViewAccountDetails>
{
    public ViewAccountDetailsPageTests()
    {
        Sut = new ViewAccountDetails(
            MockAccountService.Object,
            new FakeLinkGenerator(),
            MockCreateAccountJourneyService.Object
        )
        {
            TempData = TempData
        };
    }

    private ViewAccountDetails Sut { get; }

    [Fact]
    public async Task Get_WhenCalledWithId_LoadsTheView()
    {
        // Arrange
        var account = AccountBuilder.Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Account.Should().BeEquivalentTo(account);

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        MockAccountService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledWithSocialWorker_SetsIsSocialWorkerToTrue()
    {
        // Arrange
        var account = AccountBuilder
            .WithTypes([AccountType.EarlyCareerSocialWorker])
            .WithHasCompletedLoginAccountLinking(true)
            .Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Account.Should().BeEquivalentTo(account);
        Sut.IsSocialWorker.Should().BeTrue();
        Sut.IsAssessor.Should().BeFalse();
        Sut.HasCompletedLoginAccountLinking.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-accounts");

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        MockAccountService.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(new[] { AccountType.Assessor })]
    [InlineData(new[] { AccountType.Assessor, AccountType.Coordinator })]
    public async Task Get_WhenCalledWithAssessor_SetsIsAssessorToTrue(AccountType[] accountTypes)
    {
        // Arrange
        var account = AccountBuilder.WithTypes(accountTypes.ToImmutableList()).Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Account.Should().BeEquivalentTo(account);
        Sut.IsSocialWorker.Should().BeFalse();
        Sut.IsAssessor.Should().BeTrue();

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        MockAccountService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithAccountFound_ResendsInvitationEmailAndRedirectsToManageAccountsPage()
    {
        // Arrange
        var account = AccountBuilder.Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts");

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SendInvitationEmailAsync(account), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledAndNoAccountFound_ReturnsNotFoundResult()
    {
        // Arrange
        var id = Guid.Empty;
        MockAccountService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Account));

        // Act
        var result = await Sut.OnPostAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        MockAccountService.Verify(x => x.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledAndNoAccountFound_ReturnsNotFoundResult()
    {
        // Arrange
        var id = Guid.Empty;
        MockAccountService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Account));

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        MockAccountService.Verify(x => x.GetByIdAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledAndNoAccountTypes_ReturnsPageResult()
    {
        // Arrange
        var account = AccountBuilder.WithTypes(null).Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
