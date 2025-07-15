using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class ResetCreateOrganisationJourneyModelShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_ResetsJourney()
    {
        // Arrange
        var organisation = OrganisationBuilder.WithRegion().Build();

        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel
            {
                Organisation = organisation,
                LocalAuthorityCode = organisation.LocalAuthorityCode
            }
        );

        // Act
        Sut.ResetCreateOrganisationJourneyModel();

        // Assert
        HttpContext.Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? createOrganisationJourneyModel
        );

        createOrganisationJourneyModel.Should().BeNull();
    }
}
