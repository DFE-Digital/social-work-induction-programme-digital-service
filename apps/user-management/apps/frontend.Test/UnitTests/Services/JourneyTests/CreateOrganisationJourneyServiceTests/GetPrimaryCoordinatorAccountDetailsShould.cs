using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class GetPrimaryCoordinatorAccountDetailsShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsOrganisation()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var expected = AccountDetails.FromAccount(account);
        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel { PrimaryCoordinatorAccountDetails = expected }
        );

        // Act
        var response = Sut.GetPrimaryCoordinatorAccountDetails();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AccountDetails>();
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetPrimaryCoordinatorAccountDetails();

        // Assert
        response.Should().BeNull();
    }
}
