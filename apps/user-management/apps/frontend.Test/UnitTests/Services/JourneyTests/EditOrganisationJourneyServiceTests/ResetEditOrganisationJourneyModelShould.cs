using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class ResetEditOrganisationJourneyModelShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ResetsJourney()
    {
        // Arrange
        var organisation = OrganisationBuilder.WithRegion().Build();

        HttpContext.Session.Set(
            EditOrganisationSessionKey,
            new EditOrganisationJourneyModel
            {
                Organisation = organisation,
                PrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator
            }
        );

        // Act
        Sut.ResetEditOrganisationJourneyModel();

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey,
            out EditOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().BeNull();
    }
}
