using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Xunit;
using FluentAssertions;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class GetPrimaryCoordinatorChangeTypeShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_ReturnsChangeType()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);
        var expectedPrimaryCoordinatorChangeType = PrimaryCoordinatorChangeType.UpdateExistingCoordinator;

        var model = new EditOrganisationJourneyModel(organisation, primaryCoordinator)
        {
            PrimaryCoordinatorChangeType = expectedPrimaryCoordinatorChangeType
        };

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            model
        );

        // Act
        var response = await Sut.GetPrimaryCoordinatorChangeTypeAsync(organisation.OrganisationId!.Value);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expectedPrimaryCoordinatorChangeType);
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = await Sut.GetPrimaryCoordinatorChangeTypeAsync(Guid.Empty);

        // Assert
        response.Should().BeNull();
    }
}
