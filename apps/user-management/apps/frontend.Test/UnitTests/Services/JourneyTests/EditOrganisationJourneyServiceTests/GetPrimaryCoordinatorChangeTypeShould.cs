using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Xunit;
using FluentAssertions;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class GetPrimaryCoordinatorChangeTypeShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsChangeType()
    {
        // Arrange
        var primaryCoordinatorChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator;
        HttpContext.Session.Set(
            EditOrganisationSessionKey,
            new EditOrganisationJourneyModel { PrimaryCoordinatorChangeType = primaryCoordinatorChangeType }
        );

        // Act
        var response = Sut.GetPrimaryCoordinatorChangeType();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(primaryCoordinatorChangeType);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetPrimaryCoordinatorChangeType();

        // Assert
        response.Should().BeNull();
    }
}
