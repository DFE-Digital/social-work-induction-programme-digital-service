using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services;
using Moq;
using Notify.Interfaces;

namespace DfeSwwEcf.NotificationService.UnitTests.Services.EmailSenderTests;

public class RunAsyncShould
{
    private readonly Mock<IAsyncNotificationClient> _mockGovNotifyClient;
    private readonly EmailNotificationCommand _sut;

    public RunAsyncShould()
    {
        _mockGovNotifyClient = new();
        _sut = new(_mockGovNotifyClient.Object);
    }

    [Fact]
    public async Task WhenCalled_SendsEmailNotificationAsync()
    {
        // Arrange
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = "test@test.com",
            TemplateId = Guid.NewGuid(),
            Personalisation = new()
            {
               { "first_name", "Amala" }
            },
            Reference = "string",
            EmailReplyToId = Guid.NewGuid()
        };

        var expectedPersonalisation = new Dictionary<string, dynamic>()
        {
           { "first_name", "Amala" }
        };

        // Act
        await _sut.SendNotificationAsync(notificationRequest);

        // Assert
        _mockGovNotifyClient
            .Verify(x => x.SendEmailAsync(
                notificationRequest.EmailAddress,
                notificationRequest.TemplateId.ToString(),
                expectedPersonalisation,
                notificationRequest.Reference,
                notificationRequest.EmailReplyToId.ToString())
            , Times.Once);
        _mockGovNotifyClient.VerifyNoOtherCalls();
    }
}
