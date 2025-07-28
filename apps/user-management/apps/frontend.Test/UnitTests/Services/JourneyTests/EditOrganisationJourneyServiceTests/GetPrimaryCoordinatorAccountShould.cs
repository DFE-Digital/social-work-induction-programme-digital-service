using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class GetPrimaryCoordinatorAccountShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_ReturnsOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var expectedPrimaryCoordinator = AccountDetails.FromAccount(account);

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            new EditOrganisationJourneyModel(organisation, expectedPrimaryCoordinator)
        );

        // Act
        var response = await Sut.GetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AccountDetails>();
        response.Should().BeEquivalentTo(expectedPrimaryCoordinator);
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = await Sut.GetPrimaryCoordinatorAccountAsync(Guid.Empty);

        // Assert
        response.Should().BeNull();
    }
}
