using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.ManageOrganisationJourneyServiceTests;

public class SetOrganisationShould : ManageOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsOrganisation()
    {
        // Arrange
        var expectedOrganisation = OrganisationBuilder.Build();
        var existingOrganisation = OrganisationBuilder.Build();
        HttpContext.Session.Set(
            ManageOrganisationSessionKey,
            new ManageOrganisationJourneyModel { Organisation = existingOrganisation }
        );

        // Act
        Sut.SetOrganisation(expectedOrganisation);

        // Assert
        HttpContext.Session.TryGet(
            ManageOrganisationSessionKey,
            out ManageOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.Organisation.Should().BeEquivalentTo(expectedOrganisation);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsOrganisation()
    {
        // Arrange
        var expectedOrganisation = OrganisationBuilder.Build();

        // Act
        Sut.SetOrganisation(expectedOrganisation);

        // Assert
        HttpContext.Session.TryGet(
            ManageOrganisationSessionKey,
            out ManageOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.Organisation.Should().BeEquivalentTo(expectedOrganisation);
    }
}
