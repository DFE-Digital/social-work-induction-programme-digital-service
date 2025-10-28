using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetAccountLabelsShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true,  true, true, true,
        IsStaffLabels.IsStaffTrue, "Yes",  "Yes", "Yes"
    )]
    [InlineData(false,  false, false, false,
        IsStaffLabels.IsStaffFalse, "No", null, "No"
    )]
    [InlineData(null,  null, null, null,
        IsStaffLabels.IsStaffFalse,  null, null, null)]
    public void GetIsRegisteredWithSocialWorkEnglandLabel_ReturnsYesOnlyWhenTrue(
        bool? isStaff,
        bool? isAgency,
        bool? isStatutory,
        bool? isQualified,
        string? expectedStaffLabel,
        string? expectedAgencyLabel,
        string? expectedStatutoryLabel,
        string? expectedQualifiedLabel
    )
    {
        // Arrange
        var createAccountJourneyModel = new CreateAccountJourneyModel
        {
            IsStaff = isStaff,
            IsAgencyWorker = isAgency,
            IsStatutoryWorker = isStatutory,
            IsRecentlyQualified = isQualified
        };
        HttpContext.Session.Set(CreateAccountSessionKey, createAccountJourneyModel);

        // Act
        var accountLabels = Sut.GetAccountLabels();

        // Assert
        accountLabels.Should().NotBeNull();
        accountLabels.IsStaffLabel.Should().Be(expectedStaffLabel);
        accountLabels.IsAgencyWorkerLabel.Should().Be(expectedAgencyLabel);
        accountLabels.IsStatutoryWorkerLabel.Should().Be(expectedStatutoryLabel);
        accountLabels.IsRecentlyQualifiedLabel.Should().Be(expectedQualifiedLabel);

        VerifyAllNoOtherCall();
    }
}
