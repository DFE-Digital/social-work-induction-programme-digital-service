using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class SetPrimaryCoordinatorChangeType : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_SetsPrimaryCoordinatorChangeType()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);
        var expectedPrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator;

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            new EditOrganisationJourneyModel(organisation, primaryCoordinator)
        );

        // Act
        await Sut.SetPrimaryCoordinatorChangeTypeAsync(organisation.OrganisationId!.Value, expectedPrimaryCoordinatorChangeType);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorChangeType.Should().Be(expectedPrimaryCoordinatorChangeType);
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_SetsLocalAuthorityCode()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);
        var expectedPrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator;

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            new EditOrganisationJourneyModel(organisation, primaryCoordinator)
        );

        // Act
        await Sut.SetPrimaryCoordinatorChangeTypeAsync(organisation.OrganisationId!.Value, expectedPrimaryCoordinatorChangeType);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorChangeType.Should().Be(expectedPrimaryCoordinatorChangeType);
    }
}
