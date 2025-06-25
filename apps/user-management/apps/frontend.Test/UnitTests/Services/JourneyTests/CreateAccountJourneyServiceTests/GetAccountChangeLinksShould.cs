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
            SelectedAccountChangeLink                 = "/manage-accounts/select-account-type",
            RegisteredWithSocialWorkEnglandChangeLink = "/manage-accounts/eligibility-social-work-england",
            StatutoryWorkerChangeLink                 = "/manage-accounts/eligibility-statutory-work",
            AgencyWorkerChangeLink                    = "/manage-accounts/eligibility-agency-worker",
            RecentlyQualifiedChangeLink               = "/manage-accounts/eligibility-qualification",
            CoreDetailsChangeLink                     = "/manage-accounts/add-account-details?handler=Change",
            ProgrammeDatesChangeLink                  = "/manage-accounts/social-worker-programme-dates"
        };

        // Act
        var accountChangeLinks = Sut.GetAccountChangeLinks();

        // Assert
        accountChangeLinks.Should().NotBeNull();
        accountChangeLinks.Should().BeEquivalentTo(expectedChangeLinks);

        VerifyAllNoOtherCall();
    }
}
