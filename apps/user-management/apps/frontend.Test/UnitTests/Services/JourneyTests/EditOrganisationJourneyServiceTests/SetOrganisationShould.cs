using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class SetOrganisationShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsOrganisation()
    {
        // Arrange
        var expectedOrganisation = OrganisationBuilder.Build();
        var existingOrganisation = OrganisationBuilder.Build();
        HttpContext.Session.Set(
            EditOrganisationSessionKey,
            new CreateOrganisationJourneyModel { Organisation = existingOrganisation }
        );

        // Act
        Sut.SetOrganisation(expectedOrganisation);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey,
            out CreateOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.Organisation.Should().BeEquivalentTo(expectedOrganisation);
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
            EditOrganisationSessionKey,
            out CreateOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.Organisation.Should().BeEquivalentTo(expectedOrganisation);
    }
}
