using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsStaffLabelShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true,  IsStaffLabels.IsStaffTrue)]
    [InlineData(false, IsStaffLabels.IsStaffFalse)]
    [InlineData(null,  IsStaffLabels.IsStaffFalse)]
    public void GetIsStaffLabel_ReturnsExpectedLabel(bool? isStaff, string expectedLabel)
    {
        // Arrange
        HttpContext.Session.Set(
                CreateAccountSessionKey,
                new CreateAccountJourneyModel { IsStaff = isStaff }
            );

        // Act
        var label = Sut.GetIsStaffLabel();

        // Assert
        label.Should().Be(expectedLabel);

        VerifyAllNoOtherCall();
    }
}
