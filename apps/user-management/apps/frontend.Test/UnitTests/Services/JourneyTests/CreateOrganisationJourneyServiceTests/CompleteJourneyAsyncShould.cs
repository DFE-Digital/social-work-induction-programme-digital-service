using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using FluentAssertions;
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
        MockEmailService.Verify(x => x.SendInvitationEmailAsync(It.Is<InvitationEmailRequest>(req =>
            req.AccountId == primaryCoordinator.Id
            && req.OrganisationName == organisation.OrganisationName
            && req.IsPrimaryCoordinator == true
        )));

        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task CompleteJourneyAsync_WithNullOrganisation_ThrowsArgumentNullException()
    {
        // Arrange
        HttpContext.Session.Set(CreateOrganisationSessionKey, new CreateOrganisationJourneyModel
        {
            Organisation = null,
            PrimaryCoordinatorAccountDetails = new AccountDetails()
        });

        // Act & Assert
        await Sut.Invoking(x => x.CompleteJourneyAsync()).Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CompleteJourneyAsync_WithNullPrimaryCoordinator_ThrowsArgumentNullException()
    {
        // Arrange
        HttpContext.Session.Set(CreateOrganisationSessionKey, new CreateOrganisationJourneyModel
        {
            Organisation = OrganisationBuilder.Build(),
            PrimaryCoordinatorAccountDetails = null
        });

        // Act & Assert
        await Sut.Invoking(x => x.CompleteJourneyAsync()).Should().ThrowAsync<ArgumentNullException>();
    }
}
