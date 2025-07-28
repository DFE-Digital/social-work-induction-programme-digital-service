using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class SetOrganisationShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_SetsOrganisation()
    {
        // Arrange
        var expectedOrganisation = OrganisationBuilder.Build();
        var existingOrganisation = OrganisationBuilder.Build();
        HttpContext.Session.Set(
            EditOrganisationSessionKey(existingOrganisation.OrganisationId!.Value),
            new CreateOrganisationJourneyModel { Organisation = existingOrganisation }
        );

        // Act
        await Sut.SetOrganisationAsync(existingOrganisation.OrganisationId!.Value, expectedOrganisation);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(existingOrganisation.OrganisationId!.Value),
            out CreateOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.Organisation.Should().BeEquivalentTo(expectedOrganisation);
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_SetsOrganisation()
    {
        // Arrange
        var expectedOrganisation = OrganisationBuilder.Build();

        HttpContext.Session.Set(
            EditOrganisationSessionKey(expectedOrganisation.OrganisationId!.Value),
            new CreateOrganisationJourneyModel { Organisation = expectedOrganisation }
        );

        // Act
        await Sut.SetOrganisationAsync(expectedOrganisation.OrganisationId!.Value, expectedOrganisation);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(expectedOrganisation.OrganisationId!.Value),
            out CreateOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.Organisation.Should().BeEquivalentTo(expectedOrganisation);
    }
}
