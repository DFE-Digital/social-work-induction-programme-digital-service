using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditOrganisationJourneyServiceTests;

public class SetPrimaryCoordinatorAccountShould : EditOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsOrganisation()
    {
        // Arrange
        var expectedAccount = AccountBuilder.Build();
        var existingAccount = AccountBuilder.Build();
        HttpContext.Session.Set(
            EditOrganisationSessionKey,
            new EditOrganisationJourneyModel { PrimaryCoordinatorAccount = existingAccount }
        );

        // Act
        Sut.SetPrimaryCoordinatorAccount(expectedAccount);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey,
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorAccount.Should().BeEquivalentTo(expectedAccount);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsOrganisation()
    {
        // Arrange
        var expectedAccount = AccountBuilder.Build();

        // Act
        Sut.SetPrimaryCoordinatorAccount(expectedAccount);

        // Assert
        HttpContext.Session.TryGet(
            EditOrganisationSessionKey,
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );

        editOrganisationJourneyModel.Should().NotBeNull();
        editOrganisationJourneyModel!.PrimaryCoordinatorAccount.Should().BeEquivalentTo(expectedAccount);
    }
}
