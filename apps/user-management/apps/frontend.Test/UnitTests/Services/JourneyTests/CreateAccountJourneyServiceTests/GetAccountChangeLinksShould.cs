using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetAccountChangeLinksShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void GetAccountChangeLinks_ReturnsChangeLinks()
    {
        // Arrange
        var expectedChangeLinks = new AccountChangeLinks
        {
            UserTypeChangeLink = "/manage-accounts/select-account-type?handler=Change",
            AccountTypesChangeLink = "/manage-accounts/select-use-case?handler=Change",
            RegisteredWithSocialWorkEnglandChangeLink = "/manage-accounts/eligibility-social-work-england?handler=Change",
            StatutoryWorkerChangeLink = "/manage-accounts/eligibility-statutory-work?handler=Change",
            AgencyWorkerChangeLink = "/manage-accounts/eligibility-agency-worker?handler=Change",
            RecentlyQualifiedChangeLink = "/manage-accounts/eligibility-qualification?handler=Change",
            FirstNameChangeLink = "/manage-accounts/add-account-details?handler=Change#FirstName",
            MiddleNamesChangeLink = "/manage-accounts/add-account-details?handler=Change#MiddleNames",
            LastNameChangeLink = "/manage-accounts/add-account-details?handler=Change#LastName",
            EmailChangeLink = "/manage-accounts/add-account-details?handler=Change#Email",
            SocialWorkEnglandNumberChangeLink = "/manage-accounts/add-account-details?handler=Change#SocialWorkEnglandNumber",
            ProgrammeDatesChangeLink = "/manage-accounts/social-worker-programme-dates"
        };

        // Act
        var accountChangeLinks = Sut.GetAccountChangeLinks(false);

        // Assert
        accountChangeLinks.Should().NotBeNull();
        accountChangeLinks.Should().BeEquivalentTo(expectedChangeLinks);

        VerifyAllNoOtherCall();
    }
}
