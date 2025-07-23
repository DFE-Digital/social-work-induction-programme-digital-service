using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class SetPrimaryCoordinatorAccountDetailsShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsOrganisation()
    {
        // Arrange
        var expected = AccountBuilder.Build();
        var expectedAccount = AccountDetails.FromAccount(expected);
        var existingAccount = AccountDetails.FromAccount(AccountBuilder.Build());
        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel { PrimaryCoordinatorAccountDetails = existingAccount }
        );

        // Act
        Sut.SetPrimaryCoordinatorAccountDetails(expectedAccount);

        // Assert
        HttpContext.Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.PrimaryCoordinatorAccountDetails.Should().BeEquivalentTo(expectedAccount);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsOrganisation()
    {
        // Arrange
        var expected = AccountBuilder.Build();
        var expectedAccount = AccountDetails.FromAccount(expected);

        // Act
        Sut.SetPrimaryCoordinatorAccountDetails(expectedAccount);

        // Assert
        HttpContext.Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.PrimaryCoordinatorAccountDetails.Should().BeEquivalentTo(expectedAccount);
    }
}
