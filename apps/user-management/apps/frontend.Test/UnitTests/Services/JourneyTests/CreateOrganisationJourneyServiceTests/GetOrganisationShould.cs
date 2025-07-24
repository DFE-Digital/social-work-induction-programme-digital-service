using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class GetOrganisationShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsOrganisation()
    {
        // Arrange
        var expectedOrganisation = OrganisationBuilder.Build();
        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel { Organisation = expectedOrganisation }
        );

        // Act
        var response = Sut.GetOrganisation();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedOrganisation);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetOrganisation();

        // Assert
        response.Should().BeNull();
    }
}
