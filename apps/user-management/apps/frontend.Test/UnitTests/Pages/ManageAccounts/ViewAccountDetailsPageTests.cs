using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class ViewAccountDetailsPageTests : ManageAccountsPageTestBase<ViewAccountDetails>
{
    public ViewAccountDetailsPageTests()
    {
        Sut = new ViewAccountDetails(MockAccountService.Object, new FakeLinkGenerator())
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
        var account = AccountBuilder.WithTypes([AccountType.EarlyCareerSocialWorker]).Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Account.Should().BeEquivalentTo(account);
        Sut.IsSocialWorker.Should().BeTrue();
        Sut.IsAssessor.Should().BeFalse();

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
}
