using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetAccountLabelsShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true, true, true, true, true,
        IsStaffLabels.IsStaffTrue, "Yes", "Yes", "Yes", "Yes"
    )]
    [InlineData(false, false, false, false, false,
        IsStaffLabels.IsStaffFalse, null, "No", null, "No"
    )]
    [InlineData(null, null, null, null, null,
        IsStaffLabels.IsStaffFalse, null, null, null, null)]
    public void GetIsRegisteredWithSocialWorkEnglandLabel_ReturnsYesOnlyWhenTrue(
        bool? isStaff,
        bool? isRegistered,
        bool? isAgency,
        bool? isStatutory,
        bool? isQualified,
        string? expectedStaffLabel,
        string? expectedRegisteredLabel,
        string? expectedAgencyLabel,
        string? expectedStatutoryLabel,
        string? expectedQualifiedLabel
    )
    {
        // Arrange
        var createAccountJourneyModel = new CreateAccountJourneyModel
        {
            IsStaff = isStaff,
            IsRegisteredWithSocialWorkEngland = isRegistered,
            IsAgencyWorker = isAgency,
            IsStatutoryWorker = isStatutory,
            IsRecentlyQualified = isQualified
        };
        HttpContext.Session.Set(CreateAccountSessionKey, createAccountJourneyModel);

        // Act
        var accountLabels = Sut.GetAccountLabels();

        // Assert
        accountLabels.Should().NotBeNull();
        accountLabels!.IsStaffLabel.Should().Be(expectedStaffLabel);
        accountLabels.IsRegisteredWithSocialWorkEnglandLabel.Should().Be(expectedRegisteredLabel);
        accountLabels.IsAgencyWorkerLabel.Should().Be(expectedAgencyLabel);
        accountLabels.IsStatutoryWorkerLabel.Should().Be(expectedStatutoryLabel);
        accountLabels.IsRecentlyQualifiedLabel.Should().Be(expectedQualifiedLabel);

        VerifyAllNoOtherCall();
    }
}
