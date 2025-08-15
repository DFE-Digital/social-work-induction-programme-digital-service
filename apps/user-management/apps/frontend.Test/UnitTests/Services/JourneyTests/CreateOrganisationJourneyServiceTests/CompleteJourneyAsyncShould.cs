using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class CompleteJourneyAsyncShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_CreatesOrganisationAndInvitesPrimaryCoordinator()
    {
        // Arrange
        var primaryCoordinator = AccountBuilder.Build();
        var organisation = OrganisationBuilder.WithPrimaryCoordinatorId(primaryCoordinator.Id).Build();
        MockOrganisationService.Setup(x => x.CreateAsync(It.IsAny<Organisation>(), It.IsAny<Account>())).ReturnsAsync(organisation);
        var linkingToken = new Faker().Random.String();
        MockAuthServiceClient.MockAccountsOperations.Setup(x => x.GetLinkingTokenByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(linkingToken);
        MockAccountService.Setup(x => x.GetByIdAsync(primaryCoordinator.Id)).ReturnsAsync(primaryCoordinator);

        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel
            {
                Organisation = organisation,
                LocalAuthorityCode = organisation.LocalAuthorityCode,
                PrimaryCoordinatorAccountDetails = AccountDetails.FromAccount(primaryCoordinator)
            }
        );

        // Act
        await Sut.CompleteJourneyAsync();

        // Assert
        MockOrganisationService.Verify(x => x.CreateAsync(It.Is<Organisation>(org => org.OrganisationName == organisation.OrganisationName),
            It.Is<Account>(acc => acc.Email == primaryCoordinator.Email))
        );
        MockNotificationServiceClient.Verify(x => x.Notification.SendEmailAsync(It.Is<NotificationRequest>(req =>
            req.EmailAddress == primaryCoordinator.Email
            && req.Personalisation != null
            && req.Personalisation["name"] == primaryCoordinator.FullName
            && req.Personalisation["organisation"] == organisation.OrganisationName
            && req.Personalisation["invitation_link"] == new FakeLinkGenerator().SignInWithLinkingToken(HttpContext, linkingToken)
        )));
    }
}
