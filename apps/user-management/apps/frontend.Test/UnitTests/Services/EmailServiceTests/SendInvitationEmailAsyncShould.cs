using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.EmailServiceTests;

public class SendInvitationEmailAsyncShould : EmailServiceTestBase
{
    [Fact]
    public async Task SendInvitationEmailAsync_WhenNoHttpContext_ReturnsWithoutSendingEmail()
    {
        // Arrange
        MockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var request = new InvitationEmailRequest
        {
            AccountId = Guid.NewGuid(),
            OrganisationName = "Test Organisation"
        };

        // Act
        await Sut.SendInvitationEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        MockAuthServiceClient.Verify(x => x.Accounts, Times.Never);
        MockNotificationServiceClient.Verify(x => x.Notification, Times.Never);
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendInvitationEmailAsync_WhenAccountNotFound_ReturnsWithoutSendingEmail()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new InvitationEmailRequest
        {
            AccountId = accountId,
            OrganisationName = "Test Organisation"
        };

        MockAccountService.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync((Account?)null);

        // Act
        await Sut.SendInvitationEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        MockAuthServiceClient.Verify(x => x.Accounts, Times.Never);
        MockNotificationServiceClient.Verify(x => x.Notification, Times.Never);
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendInvitationEmailAsync_WhenAccountHasNoEmail_ReturnsWithoutSendingEmail()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new InvitationEmailRequest
        {
            AccountId = accountId,
            OrganisationName = "Test Organisation"
        };

        var account = AccountBuilder.WithEmail(null).Build();
        MockAccountService.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync(account);

        // Act
        await Sut.SendInvitationEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        MockAuthServiceClient.Verify(x => x.Accounts, Times.Never);
        MockNotificationServiceClient.Verify(x => x.Notification, Times.Never);
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendInvitationEmailAsync_WithPrimaryCoordinator_SendsEmailWithCorrectTemplate()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new InvitationEmailRequest
        {
            AccountId = accountId,
            OrganisationName = "Test Organisation",
            IsPrimaryCoordinator = true
        };

        var account = AccountBuilder.Build();
        const string linkingToken = "test-linking-token";

        MockAccountService.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync(account);
        MockAuthServiceClient.MockAccountsOperations.Setup(x => x.GetLinkingTokenByAccountIdAsync(accountId)).ReturnsAsync(linkingToken);
        NotificationRequest? capturedNotificationRequest = null;
        MockNotificationServiceClient.MockNotificationsOperations.Setup(x => x.SendEmailAsync(It.IsAny<NotificationRequest>()))
            .Callback<NotificationRequest>(req => capturedNotificationRequest = req);

        // Act
        await Sut.SendInvitationEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        MockAuthServiceClient.MockAccountsOperations.Verify(x => x.GetLinkingTokenByAccountIdAsync(accountId), Times.Once);

        MockNotificationServiceClient.MockNotificationsOperations.Verify(x => x.SendEmailAsync(It.IsAny<NotificationRequest>()), Times.Once);
        capturedNotificationRequest.Should().BeEquivalentTo(new NotificationRequest
        {
            EmailAddress = account.Email!,
            TemplateId = MockEmailTemplateOptions.Options.PrimaryCoordinatorInvitationEmail,
            Personalisation = new Dictionary<string, string>
            {
                ["name"] = account.FullName,
                ["organisation"] = request.OrganisationName,
                ["invitation_link"] = LinkGenerator.SignInWithLinkingToken(HttpContext, linkingToken)
            }
        });

        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendInvitationEmailAsync_WithoutIsPrimaryCoordinator_SendsEmailWithCorrectTemplate()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new InvitationEmailRequest
        {
            AccountId = accountId,
            OrganisationName = "Test Organisation"
        };

        var account = AccountBuilder.Build();
        const string linkingToken = "test-linking-token";

        MockAccountService.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync(account);
        MockAuthServiceClient.MockAccountsOperations.Setup(x => x.GetLinkingTokenByAccountIdAsync(accountId)).ReturnsAsync(linkingToken);
        NotificationRequest? capturedNotificationRequest = null;
        MockNotificationServiceClient.MockNotificationsOperations.Setup(x => x.SendEmailAsync(It.IsAny<NotificationRequest>()))
            .Callback<NotificationRequest>(req => capturedNotificationRequest = req);

        // Act
        await Sut.SendInvitationEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        MockAuthServiceClient.MockAccountsOperations.Verify(x => x.GetLinkingTokenByAccountIdAsync(accountId), Times.Once);

        MockNotificationServiceClient.MockNotificationsOperations.Verify(x => x.SendEmailAsync(It.Is<NotificationRequest>(req =>
            req.TemplateId == MockEmailTemplateOptions.Options.Invitation
        )), Times.Once);
        capturedNotificationRequest.Should().BeEquivalentTo(new NotificationRequest
        {
            EmailAddress = account.Email!,
            TemplateId = MockEmailTemplateOptions.Options.Invitation,
            Personalisation = new Dictionary<string, string>
            {
                ["name"] = account.FullName,
                ["organisation"] = request.OrganisationName,
                ["invitation_link"] = LinkGenerator.SignInWithLinkingToken(HttpContext, linkingToken)
            }
        });

        VerifyNoOtherCalls();
    }
}
