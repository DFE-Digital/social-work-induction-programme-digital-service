using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class LinkAccountPageTests : ManageAccountsPageTestBase<EditAccountDetails>
{
    private LinkAccount Sut { get; }

    public LinkAccountPageTests()
    {
        Sut = new LinkAccount(
            EditAccountJourneyService,
            new FakeLinkGenerator()
        )
        {
            TempData = TempData
        };
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheViewWithAccountDetails()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();

        // Act
        var result = Sut.OnGet(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.Email.Should().Be(account.Email);
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);
    }

    [Fact]
    public void Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = Sut.OnGet(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Post_WhenCalledForSocialWorkerPendingRegistration_UpdatesAccountStatusAndRedirectsToAccountDetails()
    {
        // Arrange
        var account = new AccountFaker().GenerateSocialWorkerWithNoSweNumber();
        AccountRepository.Add(account);

        // Act
        var result = Sut.OnPost(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts");

        var updatedAccount = AccountRepository.GetById(account.Id);
        updatedAccount.Should().NotBeNull();
        updatedAccount!.Status.Should().Be(AccountStatus.PendingRegistration);

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);
    }

    [Fact]
    public void Post_WhenCalledForSocialWorkerWithSweNumber_UpdatesAccountStatusAndRedirectsToAccountDetails()
    {
        // Arrange
        var account = new AccountFaker().GenerateSocialWorker();
        AccountRepository.Add(account);

        // Act
        var result = Sut.OnPost(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts");

        var updatedAccount = AccountRepository.GetById(account.Id);
        updatedAccount.Should().NotBeNull();
        updatedAccount!.Status.Should().Be(AccountStatus.Active);

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);
    }

    [Fact]
    public void Post_WhenCalledForStaff_UpdatesAccountStatusAndRedirectsToAccountDetails()
    {
        // Arrange
        var account = new AccountFaker().GenerateAccountWithTypes([AccountType.Assessor]);
        AccountRepository.Add(account);

        // Act
        var result = Sut.OnPost(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts");

        var updatedAccount = AccountRepository.GetById(account.Id);
        updatedAccount.Should().NotBeNull();
        updatedAccount!.Status.Should().Be(AccountStatus.Active);

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);
    }

    [Fact]
    public void Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = Sut.OnPost(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
