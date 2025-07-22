using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.ManageOrganisationJourneyServiceTests;

public class GetLocalAuthorityCodeShould : ManageOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsLocalAuthorityCode()
    {
        // Arrange
        var expectedLocalAuthorityCode = new Faker().Random.Int();
        HttpContext.Session.Set(
            ManageOrganisationSessionKey,
            new ManageOrganisationJourneyModel { LocalAuthorityCode = expectedLocalAuthorityCode }
        );

        // Act
        var response = Sut.GetLocalAuthorityCode();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expectedLocalAuthorityCode);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetLocalAuthorityCode();

        // Assert
        response.Should().BeNull();
    }
}
