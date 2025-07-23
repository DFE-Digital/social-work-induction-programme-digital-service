using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class SetLocalAuthorityCodeShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsLocalAuthorityCode()
    {
        // Arrange
        var expectedLocalAuthorityCode = new Faker().Random.Int();
        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel { LocalAuthorityCode = new Faker().Random.Int() }
        );

        // Act
        Sut.SetLocalAuthorityCode(expectedLocalAuthorityCode);

        // Assert
        HttpContext.Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? manageOrganisationJourneyModel
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
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.LocalAuthorityCode.Should().Be(expectedLocalAuthorityCode);
    }
}
