using System.Net;
using Dfe.Sww.Ecf.Frontend.Configuration.Notification;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.EmailServiceTests.LinkingEmailServiceTests;

public class LinkAccountShould : LinkingEmailServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CallsNotificationClient_ReturnsTrue()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);
        var linkTemplateId = Guid.NewGuid();

        var expectedRequest = new NotificationRequest
        {
            EmailAddress = accountDetails.Email!,
            TemplateId = linkTemplateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", accountDetails.FullName },
                { "organisation", "TEST ORGANISATION" }, // TODO Retrieve this value when we can
                { "coordinator name", UserConstants.UserName },
                { "coordinator email", UserConstants.UserEmail }
            }
        };

        MockEmailTemplateOptions
            .Setup(x => x.Value)
            .Returns(
                new EmailTemplateOptions
                {
                    Roles = new Dictionary<string, RoleEmailTemplateConfiguration>
                    {
                        {
                            account.Types!.Min().ToString(),
                            new RoleEmailTemplateConfiguration
                            {
                                Invitation = default,
                                Welcome = default,
                                Pause = default,
                                Unpause = default,
                                Link = linkTemplateId,
                                Unlink = default
                            }
                        }
                    }
                }
            );

        MockNotificationClient
            .Setup(x =>
                x.Notification.SendEmailAsync(MoqHelpers.ShouldBeEquivalentTo(expectedRequest))
            )
            .ReturnsAsync(new NotificationResponse { StatusCode = HttpStatusCode.OK });

        // Act
        var response = await Sut.LinkAccountAsync(
            accountDetails,
            account.Types,
            UserConstants.UserName,
            UserConstants.UserEmail
        );

        // Assert
        response.Should().BeTrue();

        MockNotificationClient.Verify(
            x => x.Notification.SendEmailAsync(MoqHelpers.ShouldBeEquivalentTo(expectedRequest)),
            Times.Once
        );

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalled_WithNullParameters_ReturnsPerson()
    {
        // Act
        var response = await Sut.LinkAccountAsync(null, null, null, null);

        // Assert
        response.Should().BeFalse();

        VerifyAllNoOtherCalls();
    }
}
