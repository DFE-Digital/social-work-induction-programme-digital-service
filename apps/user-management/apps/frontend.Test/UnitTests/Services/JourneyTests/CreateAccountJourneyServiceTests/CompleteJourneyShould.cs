using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class CompleteJourneyShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CompletesJourney()
    {
        // Arrange
        var account = AccountBuilder.WithId(Guid.Empty).Build();

        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel
            {
                AccountDetails = AccountDetails.FromAccount(account),
                AccountTypes = account.Types,
                IsStaff = account.IsStaff
            }
        );

        var expected = account with { Id = Guid.NewGuid() };
        MockAccountService.Setup(x => x.CreateAsync(It.IsAny<Account>())).ReturnsAsync(expected);

        const string expectedLinkingToken = "LinkingTokenString";
        MockAuthServiceClient
            .MockAccountsOperations.Setup(x => x.GetLinkingTokenByAccountIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedLinkingToken);

        var invitationEmailTemplateId = MockEmailTemplateOptions
            .EmailTemplateOptions
            .Roles[expected.Types!.Min().ToString()]
            .Invitation;
        var expectedNotificationRequest = new NotificationRequest
        {
            EmailAddress = expected.Email!,
            TemplateId = invitationEmailTemplateId,
            Personalisation = new Dictionary<string, string>
            {
                { "name", account.FullName },
                { "organisation", "TEST ORGANISATION" }, // TODO Retrieve this value when we can
                {
                    "invitation_link",
                    new FakeLinkGenerator().SignInWithLinkingToken(
                        HttpContext,
                        expectedLinkingToken
                    )
                }
            }
        };

        // Act
        var result = await Sut.CompleteJourneyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Account>();
        result.Should().BeEquivalentTo(expected);
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );
        createAccountJourneyModel.Should().BeNull();

        MockAccountService.Verify(
            x =>
                x.CreateAsync(
                    It.Is<Account>(acc =>
                        acc.Id == account.Id
                        && acc.FullName == account.FullName
                        && acc.Email == account.Email
                        && acc.IsStaff == account.IsStaff
                        && acc.Types != null
                        && acc.Types.SequenceEqual(account.Types!)
                        && acc.SocialWorkEnglandNumber == account.SocialWorkEnglandNumber
                    )
                ),
            Times.Once
        );
        MockAuthServiceClient.MockAccountsOperations.Verify(
            x => x.GetLinkingTokenByAccountIdAsync(expected.Id),
            Times.Once()
        );
        MockNotificationServiceClient.MockNotificationsOperations.Verify(
            x => x.SendEmailAsync(MoqHelpers.ShouldBeEquivalentTo(expectedNotificationRequest)),
            Times.Once
        );
        MockEmailTemplateOptions.Verify(x => x.Value, Times.Once);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenCalled_WithBlankEmail_DoesNotSendEmail()
    {
        // Arrange
        var account = AccountBuilder.WithId(Guid.Empty).WithEmail(null).Build();

        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel
            {
                AccountDetails = AccountDetails.FromAccount(account),
                AccountTypes = account.Types,
                IsStaff = account.IsStaff
            }
        );

        var expected = account with { Id = Guid.NewGuid() };
        MockAccountService.Setup(x => x.CreateAsync(It.IsAny<Account>())).ReturnsAsync(expected);

        const string expectedLinkingToken = "LinkingTokenString";
        MockAuthServiceClient
            .MockAccountsOperations.Setup(x => x.GetLinkingTokenByAccountIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedLinkingToken);

        // Act
        var result = await Sut.CompleteJourneyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Account>();
        result.Should().BeEquivalentTo(expected);
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );
        createAccountJourneyModel.Should().BeNull();

        MockAccountService.Verify(
            x =>
                x.CreateAsync(
                    It.Is<Account>(acc =>
                        acc.Id == account.Id
                        && acc.FullName == account.FullName
                        && acc.Email == account.Email
                        && acc.IsStaff == account.IsStaff
                        && acc.Types != null
                        && acc.Types.SequenceEqual(account.Types!)
                        && acc.SocialWorkEnglandNumber == account.SocialWorkEnglandNumber
                    )
                ),
            Times.Once
        );
        MockAuthServiceClient.MockAccountsOperations.Verify(
            x => x.GetLinkingTokenByAccountIdAsync(It.IsAny<Guid>()),
            Times.Never()
        );
        MockNotificationServiceClient.MockNotificationsOperations.Verify(
            x => x.SendEmailAsync(It.IsAny<NotificationRequest>()),
            Times.Never
        );
        MockEmailTemplateOptions.Verify(x => x.Value, Times.Never);

        VerifyAllNoOtherCall();
    }
}
