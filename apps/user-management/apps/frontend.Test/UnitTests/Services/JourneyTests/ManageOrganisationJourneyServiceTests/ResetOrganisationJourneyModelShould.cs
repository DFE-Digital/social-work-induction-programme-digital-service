using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.ManageOrganisationJourneyServiceTests;

public class ResetOrganisationJourneyModelShould : ManageOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ResetsJourney()
    {
        // Arrange
        var organisation = OrganisationBuilder.WithRegion().Build();

        HttpContext.Session.Set(
            ManageOrganisationSessionKey,
            new ManageOrganisationJourneyModel
            {
                Organisation = organisation,
                LocalAuthorityCode = organisation.LocalAuthorityCode
            }
        );

        // Act
        Sut.ResetOrganisationJourneyModel();

        // Assert
        HttpContext.Session.TryGet(
            ManageOrganisationSessionKey,
            out ManageOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().BeNull();
    }
}
