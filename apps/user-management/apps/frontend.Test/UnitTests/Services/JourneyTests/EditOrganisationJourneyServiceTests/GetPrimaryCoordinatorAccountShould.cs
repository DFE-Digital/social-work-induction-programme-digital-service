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
    public void WhenCalled_WithExistingSessionData_ReturnsOrganisation()
    {
        // Arrange
        var expected = AccountBuilder.Build();
        HttpContext.Session.Set(
            EditOrganisationSessionKey,
            new EditOrganisationJourneyModel { PrimaryCoordinatorAccount = expected }
        );

        // Act
        var response = Sut.GetPrimaryCoordinatorAccount();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<Account>();
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetPrimaryCoordinatorAccount();

        // Assert
        response.Should().BeNull();
    }
}
