using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class CompleteJourneyAsyncShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithIsOrganisationUpdate_UpdatesOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.WithPrimaryCoordinatorId(Guid.Empty).Build();
        var primaryCoordinator = AccountBuilder.Build();
        var primaryCoordinatorDetails = AccountDetails.FromAccount(primaryCoordinator);
        var organisationId = organisation.OrganisationId!.Value;

        MockOrganisationService.Setup(x => x.UpdateOrganisationAsync(It.IsAny<Organisation>())).ReturnsAsync(organisation);

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisationId),
            new EditOrganisationJourneyModel(organisation, primaryCoordinatorDetails) { IsOrganisationUpdate = true }
        );

        // Act
        await Sut.CompleteJourneyAsync(organisationId);

        // Assert
        MockOrganisationService.Verify(x => x.UpdateOrganisationAsync(It.Is<Organisation>(org => org.OrganisationId == organisation.OrganisationId!.Value))
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task WhenCalled_WithPrimaryCoordinatorChangeTypeNotNull_CallsAccountServiceUpdate()
    {
        var organisation = OrganisationBuilder.WithPrimaryCoordinatorId(Guid.Empty).Build();
        var primaryCoordinator = AccountBuilder.Build();
        var primaryCoordinatorDetails = AccountDetails.FromAccount(primaryCoordinator);
        var organisationId = organisation.OrganisationId!.Value;

        MockOrganisationService.Setup(x => x.UpdateOrganisationAsync(It.IsAny<Organisation>())).ReturnsAsync(organisation);

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisationId),
            new EditOrganisationJourneyModel(organisation, primaryCoordinatorDetails) { PrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator }
        );

        // Act
        await Sut.CompleteJourneyAsync(organisationId);

        // Assert
        MockAccountService.Verify(x => x.UpdateAsync(It.Is<Account>(acc => acc.Email == primaryCoordinator.Email))
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task CompleteJourneyAsync_WithNullOrganisation_ThrowsArgumentNullException()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var primaryCoordinator = AccountBuilder.Build();
        var primaryCoordinatorDetails = AccountDetails.FromAccount(primaryCoordinator);

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisationId),
            new EditOrganisationJourneyModel(
                organisation: null!,
                primaryCoordinatorAccount: primaryCoordinatorDetails
            )
            {
                IsOrganisationUpdate = true
            });

        // Act & Assert
        await Sut
            .Invoking(x => x.CompleteJourneyAsync(organisationId))
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CompleteJourneyAsync_WithNullPrimaryCoordinator_ThrowsArgumentNullException()
    {
        // Arrange
        var organisation = OrganisationBuilder.WithPrimaryCoordinatorId(Guid.Empty).Build();
        var organisationId = organisation.OrganisationId!.Value;

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisationId),
            new EditOrganisationJourneyModel(
                organisation: organisation,
                primaryCoordinatorAccount: null!
            )
            {
                PrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator
            });

        // Act & Assert
        await Sut
            .Invoking(x => x.CompleteJourneyAsync(organisationId))
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }
}
