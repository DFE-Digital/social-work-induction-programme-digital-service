using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
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
            MockOrganisationService.Object,
            new FakeLinkGenerator(),
            MockEmailService.Object,
            MockEditAccountJourneyService.Object
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
    public async Task OnGetNewAsync_WhenCalled_ResetsModelAndRedirectsToViewDetails()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Sut.OnGetNewAsync(id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Should().NotBeNull();
        result.Url.Should().Be($"/manage-accounts/view-account-details/{id}");

        MockEditAccountJourneyService.Verify(x => x.ResetEditAccountJourneyModelAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithAccountFound_ResendsInvitationEmailAndRedirectsToManageAccountsPage()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var organisation = OrganisationBuilder.Build();

        MockAccountService.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);
        MockOrganisationService.Setup(x => x.GetByIdAsync(organisation.OrganisationId!.Value)).ReturnsAsync(organisation);
        MockEmailService.Setup(x => x.SendInvitationEmailAsync(It.IsAny<InvitationEmailRequest>()))
            .Returns(Task.CompletedTask);

        Sut.OrganisationId = organisation.OrganisationId!.Value;

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts");

        var notificationType = (NotificationBannerType?)TempData["NotificationType"];
        notificationType.Should().Be(NotificationBannerType.Success);

        var notificationHeader = TempData["NotificationHeader"]?.ToString();
        notificationHeader.Should().Be("An invitation to register has been resent");

        var notificationMessage = TempData["NotificationMessage"]?.ToString();
        notificationMessage.Should().Be($"A new invitation to register has been sent to {account.FullName}, {account.Email}");

        MockAccountService.Verify(x => x.GetByIdAsync(account.Id), Times.Once);
        MockOrganisationService.Verify(x => x.GetByIdAsync(organisation.OrganisationId!.Value), Times.Once);
        MockEmailService.Verify(x => x.SendInvitationEmailAsync(It.Is<InvitationEmailRequest>(req =>
                req.AccountId == account.Id &&
                req.OrganisationName == organisation.OrganisationName)),
            Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledAndNoAccountFound_ReturnsNotFoundResult()
    {
        // Arrange
        var id = Guid.Empty;
        var orgId = Guid.Empty;
        MockAccountService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Account));
        MockOrganisationService.Setup(x => x.GetByIdAsync(orgId)).ReturnsAsync(default(Organisation));

        Sut.OrganisationId = orgId;

        // Act
        var result = await Sut.OnPostAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        MockAccountService.Verify(x => x.GetByIdAsync(id), Times.Once);
        MockOrganisationService.Verify(x => x.GetByIdAsync(orgId), Times.Once);
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
