using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class SetPrimaryCoordinatorAccountShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public async Task WhenCalled_WithExistingSessionData_SetsOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var expectedAccount = AccountDetails.FromAccount(AccountBuilder.Build());
        var existingAccount = AccountDetails.FromAccount(AccountBuilder.Build());

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            new EditOrganisationJourneyModel(organisation, existingAccount)
        );

        // Act
        await Sut.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, expectedAccount);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorAccount.Should().BeEquivalentTo(expectedAccount);
    }

    [Fact]
    public async Task WhenCalled_WithBlankSession_SetsOrganisation()
    {
        // Arrange
        var organisation = OrganisationBuilder.Build();
        var expectedAccount = AccountDetails.FromAccount(AccountBuilder.Build());

        HttpContext.Session.Set(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            new EditOrganisationJourneyModel(new Organisation(), new AccountDetails())
        );

        // Act
        await Sut.SetPrimaryCoordinatorAccountAsync(organisation.OrganisationId!.Value, expectedAccount);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey(organisation.OrganisationId!.Value),
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorAccount.Should().BeEquivalentTo(expectedAccount);
    }
}
