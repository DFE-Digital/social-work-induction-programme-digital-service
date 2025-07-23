using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class SetPrimaryCoordinatorChangeType : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsPrimaryCoordinatorChangeType()
    {
        // Arrange
        var expectedPrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator;
        HttpContext.Session.Set(
            EditOrganisationSessionKey,
            new EditOrganisationJourneyModel { PrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator }
        );

        // Act
        Sut.SetPrimaryCoordinatorChangeType(expectedPrimaryCoordinatorChangeType);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey,
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorChangeType.Should().Be(expectedPrimaryCoordinatorChangeType);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsLocalAuthorityCode()
    {
        // Arrange
        var expectedPrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator;

        // Act
        Sut.SetPrimaryCoordinatorChangeType(expectedPrimaryCoordinatorChangeType);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey,
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorChangeType.Should().Be(expectedPrimaryCoordinatorChangeType);
    }
}
