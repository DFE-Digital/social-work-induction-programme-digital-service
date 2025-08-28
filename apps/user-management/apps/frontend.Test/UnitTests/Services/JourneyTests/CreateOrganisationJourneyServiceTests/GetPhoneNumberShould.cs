using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class GetPhoneNumberShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_ReturnsPhoneNumber()
    {
        // Arrange
        var expectedPhoneNumber = new Faker().Phone.PhoneNumber("+447### ######");
        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel { PhoneNumber = expectedPhoneNumber }
        );

        // Act
        var response = Sut.GetPhoneNumber();

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expectedPhoneNumber);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_ReturnsNull()
    {
        // Act
        var response = Sut.GetPhoneNumber();

        // Assert
        response.Should().BeNull();
    }
}
