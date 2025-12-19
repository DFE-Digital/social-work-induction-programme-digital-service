using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Xunit;
using FluentAssertions;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class GetIsOrganisationUpdateShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_ReturnsIsOrganisationUpdateValue()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);

        var model = new EditOrganisationJourneyModel(organisation, primaryCoordinator)
        {
            IsOrganisationUpdate = true
        };

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            model
        );

        // Act
        var response = await Sut.GetIsOrganisationUpdateAsync(organisation.OrganisationId!.Value);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeTrue();
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = await Sut.GetIsOrganisationUpdateAsync(Guid.Empty);

        // Assert
        response.Should().BeNull();
    }
}
