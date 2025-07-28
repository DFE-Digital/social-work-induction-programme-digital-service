using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
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
        var account = AccountBuilder.Build();
        var expectedPrimaryCoordinator = AccountDetails.FromAccount(account);

        var model = new EditOrganisationJourneyModel(organisation, expectedPrimaryCoordinator)
        {
            PrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator
        };

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            model
        );

        // Act
        Sut.ResetEditOrganisationJourneyModel(organisation.OrganisationId!.Value);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            out EditOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().BeNull();
    }
}
