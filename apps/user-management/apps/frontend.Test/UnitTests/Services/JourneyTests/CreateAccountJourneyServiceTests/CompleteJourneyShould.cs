using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class CompleteJourneyShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(new[] { AccountType.Administrator }, MoodleRoles.Manager)]
    [InlineData(new[] { AccountType.Coordinator }, MoodleRoles.Manager)]
    [InlineData(new[] { AccountType.Assessor }, MoodleRoles.Teacher)]
    [InlineData(new[] { AccountType.EarlyCareerSocialWorker }, MoodleRoles.Student)]
    [InlineData(new[] { AccountType.Coordinator, AccountType.Assessor }, MoodleRoles.Manager)]
    public async Task WhenCalled_CompletesJourney(AccountType[] accountTypes, MoodleRoles moodleRole)
    {
        // Arrange
        var account = AccountBuilder.WithId(Guid.Empty).WithTypes(accountTypes.ToImmutableList()).Build();
        var organisation = OrganisationBuilder.Build();
        var externalUserId = 100;
        var createJourneyModel = new CreateAccountJourneyModel
        {
            AccountDetails = AccountDetails.FromAccount(account),
            AccountTypes = account.Types,
            IsStaff = account.IsStaff
        };

        HttpContext.Session.Set(
            CreateAccountSessionKey,
            createJourneyModel
        );

        var expected = account with { Id = Guid.NewGuid() };
        MockOrganisationService.Setup(x => x.GetByIdAsync(organisation.OrganisationId)).ReturnsAsync(organisation);
        MockMoodleService.Setup(x => x.CreateUserAsync(It.IsAny<Account>())).ReturnsAsync(externalUserId);
        MockMoodleService.Setup(x => x.EnrolUserAsync(externalUserId, organisation.ExternalOrganisationId!.Value, moodleRole));
        MockAccountService.Setup(x => x.CreateAsync(It.IsAny<Account>(), It.IsAny<Guid?>())).ReturnsAsync(expected);
        InvitationEmailRequest? capturedEmailRequest = null;
        MockEmailService.Setup(x => x.SendInvitationEmailAsync(It.IsAny<InvitationEmailRequest>())).Callback<InvitationEmailRequest>(req => capturedEmailRequest = req);
        MockFeatureFlags.SetupGet(x => x.Value).Returns(new FeatureFlags { EnableMoodleIntegration = true });

        // Act
        var result = await Sut.CompleteJourneyAsync(organisation.OrganisationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Account>();
        result.Should().BeEquivalentTo(expected);
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );
        createAccountJourneyModel.Should().BeNull();

        MockOrganisationService.Verify(x => x.GetByIdAsync(organisation.OrganisationId), Times.Once);
        MockMoodleService.Verify(x => x.CreateUserAsync(It.Is<Account>(acc => acc.Email == account.Email)), Times.Once);
        MockMoodleService.Verify(x => x.EnrolUserAsync(externalUserId, organisation.ExternalOrganisationId!.Value, moodleRole), Times.Once);

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
                    ),
                    It.IsAny<Guid?>()
                ),
            Times.Once
        );
        MockEmailService.Verify(x =>
            x.SendInvitationEmailAsync(It.IsAny<InvitationEmailRequest>()), Times.Once);
        capturedEmailRequest.Should().BeEquivalentTo(new InvitationEmailRequest
        {
            AccountId = expected.Id,
            OrganisationName = organisation.OrganisationName
        });

        VerifyAllNoOtherCall();
    }
}
