using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.EmailServiceTests;

public class SendWelcomeEmailAsyncShould : EmailServiceTestBase
{
    [Fact]
    public async Task SendWelcomeEmailAsync_WhenEmailNull_ReturnsWithoutSendingEmail()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new WelcomeEmailRequest
        {
            AccountId = accountId
        };

        var account = AccountBuilder.WithEmail(null).Build();
        MockAccountService.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync(account);

        // Act
        await Sut.SendWelcomeEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        MockAuthServiceClient.Verify(x => x.Accounts, Times.Never);
        MockNotificationServiceClient.Verify(x => x.Notification, Times.Never);
        var log = MockLogger.Invocations.FirstOrDefault(i => i.Method.Name == nameof(ILogger.Log));
        log.Should().NotBeNull();
        log!.Arguments[2].ToString().Should().Contain("Email is required to send welcome email");
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WhenNameEmpty_ReturnsWithoutSendingEmail()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new WelcomeEmailRequest
        {
            AccountId = accountId
        };

        var account = AccountBuilder.WithFirstName("").WithLastName("").WithMiddleNames("").Build();
        MockAccountService.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync(account);

        // Act
        await Sut.SendWelcomeEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        MockAuthServiceClient.Verify(x => x.Accounts, Times.Never);
        MockNotificationServiceClient.Verify(x => x.Notification, Times.Never);
        var log = MockLogger.Invocations.FirstOrDefault(i => i.Method.Name == nameof(ILogger.Log));
        log.Should().NotBeNull();
        log!.Arguments[2].ToString().Should().Contain("Full name is required to send welcome email");
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_WithCoordinator_SendsEmailWithCorrectRequest()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new WelcomeEmailRequest
        {
            AccountId = accountId
        };

        var account = AccountBuilder.WithTypes([AccountType.Coordinator]).Build();

        MockAccountService.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync(account);
        NotificationRequest? capturedNotificationRequest = null;
        MockNotificationServiceClient.MockNotificationsOperations.Setup(x => x.SendEmailAsync(It.IsAny<NotificationRequest>()))
            .Callback<NotificationRequest>(req => capturedNotificationRequest = req);

        // Act
        await Sut.SendWelcomeEmailAsync(request);

        // Assert
        MockAccountService.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        MockNotificationServiceClient.MockNotificationsOperations.Verify(x => x.SendEmailAsync(It.IsAny<NotificationRequest>()), Times.Once);
        capturedNotificationRequest.Should().BeEquivalentTo(new NotificationRequest
        {
            EmailAddress = account.Email!,
            TemplateId = MockEmailTemplateOptions.Options.Welcome,
            Personalisation = new Dictionary<string, string>
            {
                ["name"] = account.FullName,
                ["ECSW"] = "no",
                ["coordinator"] = "yes",
                ["assessor"] = "no"
            }
        });

        VerifyNoOtherCalls();
    }
}

