using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateOrganisationJourneyServiceTests;

public class SetPhoneNumberShould : CreateOrganisationJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsPhoneNumber()
    {
        // Arrange
        var expectedPhoneNumber = new Faker().Phone.PhoneNumber("+447### ######");
        HttpContext.Session.Set(
            CreateOrganisationSessionKey,
            new CreateOrganisationJourneyModel { PhoneNumber = new Faker().Phone.PhoneNumber("+447### ######") }
        );

        // Act
        Sut.SetPhoneNumber(expectedPhoneNumber);

        // Assert
        HttpContext.Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? createOrganisationJourneyModel
        );

        createOrganisationJourneyModel.Should().NotBeNull();
        createOrganisationJourneyModel!.PhoneNumber.Should().Be(expectedPhoneNumber);
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsLocalAuthorityCode()
    {
        // Arrange
        var expectedPhoneNumber = new Faker().Phone.PhoneNumber("+447### ######");

        // Act
        Sut.SetPhoneNumber(expectedPhoneNumber);

        // Assert
        HttpContext.Session.TryGet(
            CreateOrganisationSessionKey,
            out CreateOrganisationJourneyModel? manageOrganisationJourneyModel
        );

        manageOrganisationJourneyModel.Should().NotBeNull();
        manageOrganisationJourneyModel!.PhoneNumber.Should().Be(expectedPhoneNumber);
    }
}
