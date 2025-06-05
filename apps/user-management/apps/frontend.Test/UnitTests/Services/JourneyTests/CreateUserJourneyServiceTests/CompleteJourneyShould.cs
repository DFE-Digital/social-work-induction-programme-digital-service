using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateUserJourneyServiceTests;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class CompleteJourneyShould : CreateUserJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CompletesJourney()
    {
        // Arrange
        var user = UserBuilder.WithId(Guid.Empty).Build();

        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel
            {
                UserDetails = UserDetails.FromUser(user),
                UserTypes = user.Types,
                IsStaff = user.IsStaff
            }
        );

        var expected = user with { Id = Guid.NewGuid() };
        MockUserService.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(expected);

        const string expectedLinkingToken = "LinkingTokenString";
        MockAuthServiceClient
            .MockUsersOperations.Setup(x => x.GetLinkingTokenByAccountIdAsync(It.IsAny<Guid>()))
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
                { "name", user.FullName },
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
        result.Should().BeOfType<User>();
        result.Should().BeEquivalentTo(expected);
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );
        createUserJourneyModel.Should().BeNull();

        MockUserService.Verify(
            x =>
                x.CreateAsync(
                    It.Is<User>(acc =>
                        acc.Id == user.Id
                        && acc.FullName == user.FullName
                        && acc.Email == user.Email
                        && acc.IsStaff == user.IsStaff
                        && acc.Types != null
                        && acc.Types.SequenceEqual(user.Types!)
                        && acc.SocialWorkEnglandNumber == user.SocialWorkEnglandNumber
                    )
                ),
            Times.Once
        );
        MockAuthServiceClient.MockUsersOperations.Verify(
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
        var user = UserBuilder.WithId(Guid.Empty).WithEmail(null).Build();

        HttpContext.Session.Set(
            CreateUserSessionKey,
            new CreateUserJourneyModel
            {
                UserDetails = UserDetails.FromUser(user),
                UserTypes = user.Types,
                IsStaff = user.IsStaff
            }
        );

        var expected = user with { Id = Guid.NewGuid() };
        MockUserService.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(expected);

        const string expectedLinkingToken = "LinkingTokenString";
        MockAuthServiceClient
            .MockUsersOperations.Setup(x => x.GetLinkingTokenByAccountIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedLinkingToken);

        // Act
        var result = await Sut.CompleteJourneyAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<User>();
        result.Should().BeEquivalentTo(expected);
        HttpContext.Session.TryGet(
            CreateUserSessionKey,
            out CreateUserJourneyModel? createUserJourneyModel
        );
        createUserJourneyModel.Should().BeNull();

        MockUserService.Verify(
            x =>
                x.CreateAsync(
                    It.Is<User>(acc =>
                        acc.Id == user.Id
                        && acc.FullName == user.FullName
                        && acc.Email == user.Email
                        && acc.IsStaff == user.IsStaff
                        && acc.Types != null
                        && acc.Types.SequenceEqual(user.Types!)
                        && acc.SocialWorkEnglandNumber == user.SocialWorkEnglandNumber
                    )
                ),
            Times.Once
        );
        MockAuthServiceClient.MockUsersOperations.Verify(
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
