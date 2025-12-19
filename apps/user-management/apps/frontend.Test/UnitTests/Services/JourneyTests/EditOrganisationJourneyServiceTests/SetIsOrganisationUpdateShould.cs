using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class SetIsOrganisationUpdateShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_SetsIsOrganisationUpdate()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            new EditOrganisationJourneyModel(organisation, primaryCoordinator)
        );

        // Act
        await Sut.SetIsOrganisationUpdateAsync(organisation.OrganisationId!.Value, true);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.IsOrganisationUpdate.Should().BeTrue();
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_ThrowsException()
    {
        var organisation = OrganisationBuilder.Build();
        var account = AccountBuilder.Build();
        var primaryCoordinator = AccountDetails.FromAccount(account);

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            new EditOrganisationJourneyModel(organisation, primaryCoordinator)
        );

        // Act
        await Sut.SetIsOrganisationUpdateAsync(organisation.OrganisationId.Value, true);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(organisation.OrganisationId.Value),
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.IsOrganisationUpdate.Should().BeTrue();
    }
}
