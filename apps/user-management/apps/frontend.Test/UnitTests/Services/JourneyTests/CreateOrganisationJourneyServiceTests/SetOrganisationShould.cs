using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class SetOrganisationShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsOrganisation()
    {
        // Arrange
        var expectedOrganisation = OrganisationBuilder.Build();
        var existingOrganisation = OrganisationBuilder.Build();
        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel { Organisation = existingOrganisation }
        );

        // Act
        Sut.SetOrganisation(expectedOrganisation);

        // Assert
        HttpContext.Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? manageOrganisationJourneyModel
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
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? createOrganisationJourneyModel
        );

        createOrganisationJourneyModel.Should().NotBeNull();
        createOrganisationJourneyModel!.Organisation.Should().BeEquivalentTo(expectedOrganisation);
    }
}
