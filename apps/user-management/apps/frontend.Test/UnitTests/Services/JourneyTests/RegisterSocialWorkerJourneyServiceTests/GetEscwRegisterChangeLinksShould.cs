using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.RegisterSocialWorkerJourneyServiceTests;

public class GetEscwRegisterChangeLinks : RegisterSocialWorkerJourneyServiceTestBase
{
    [Theory]
    [InlineData(EthnicGroup.White)]
    [InlineData(EthnicGroup.MixedOrMultipleEthnicGroups)]
    [InlineData(EthnicGroup.AsianOrAsianBritish)]
    [InlineData(EthnicGroup.BlackAfricanCaribbeanOrBlackBritish)]
    [InlineData(EthnicGroup.OtherEthnicGroup)]
    [InlineData(EthnicGroup.PreferNotToSay)]
    public void WhenCalled_ReturnChangeLinks(EthnicGroup ethnicGroup)
    {
        // Arrange
        var changeLinks = new EscwRegisterChangeLinks
        {
            DateOfBirthChangeLink = "/social-worker-registration/select-date-of-birth?handler=Change",
            UserSexChangeLink = "/social-worker-registration/select-sex-and-gender-identity?handler=Change",
            GenderIdentityChangeLink = "/social-worker-registration/select-sex-and-gender-identity?handler=Change",
            EthnicGroupChangeLink = "/social-worker-registration/select-ethnic-group?handler=Change",
            EthnicGroupingChangeLink = ethnicGroup switch
            {
                EthnicGroup.White => "/social-worker-registration/select-ethnic-group/white?handler=Change",
                EthnicGroup.MixedOrMultipleEthnicGroups => "/social-worker-registration/select-ethnic-group/mixed-or-multiple-ethnic-groups?handler=Change",
                EthnicGroup.AsianOrAsianBritish => "/social-worker-registration/select-ethnic-group/asian-or-asian-british?handler=Change",
                EthnicGroup.BlackAfricanCaribbeanOrBlackBritish => "/social-worker-registration/select-ethnic-group/black-african-caribbean-or-black-british?handler=Change",
                EthnicGroup.OtherEthnicGroup => "/social-worker-registration/select-ethnic-group/other-ethnic-group?handler=Change",
                EthnicGroup.PreferNotToSay => "/social-worker-registration/select-ethnic-group?handler=Change",
                _ => "social-worker-registration/select-ethnic-group?handler=Change"
            },
            DisabilityChangeLink = "/social-worker-registration/select-disability?handler=Change",
            SocialWorkEnglandRegistrationChangeLink = "/social-worker-registration/select-social-work-england-registration-date?handler=Change",
            HighestQualificationChangeLink = "/social-worker-registration/select-highest-qualification?handler=Change",
            SocialWorkQualificationEndYearChangeLink = "/social-worker-registration/select-social-work-qualification-end-year?handler=Change",
            RouteIntoSocialWorkChangeLink = "/social-worker-registration/select-route-into-social-work?handler=Change",
        };

        // Act
        var response = Sut.GetEscwRegisterChangeLinks(ethnicGroup);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(changeLinks);

        VerifyAllNoOtherCall();
    }
}
