using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.ManageOrganisationJourneyServiceTests;

public class SetLocalAuthorityCodeShould : ManageOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsLocalAuthorityCode()
    {
        // Arrange
        var expectedLocalAuthorityCode = new Faker().Random.Int();
        HttpContext.Session.Set(
            ManageOrganisationSessionKey,
            new ManageOrganisationJourneyModel { LocalAuthorityCode = new Faker().Random.Int() }
        );

        // Act
        Sut.SetLocalAuthorityCode(expectedLocalAuthorityCode);

        // Assert
        HttpContext.Session.TryGet(
            ManageOrganisationSessionKey,
            out ManageOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.LocalAuthorityCode.Should().Be(expectedLocalAuthorityCode);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsLocalAuthorityCode()
    {
        // Arrange
        var expectedLocalAuthorityCode = new Faker().Random.Int();

        // Act
        Sut.SetLocalAuthorityCode(expectedLocalAuthorityCode);

        // Assert
        HttpContext.Session.TryGet(
            ManageOrganisationSessionKey,
            out ManageOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.LocalAuthorityCode.Should().Be(expectedLocalAuthorityCode);
    }
}
