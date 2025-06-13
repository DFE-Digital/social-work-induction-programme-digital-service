using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsQualifiedWithin3YearsLabelShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true,  "Yes")]
    [InlineData(false, "No")]
    [InlineData(null,  null)]
    public void GetIsQualifiedWithin3YearsLabelLabel_ReturnsExpectedLabel(bool? isQualified,
        string? expectedLabel)
    {
        // Arrange
        var journeyModel = new CreateAccountJourneyModel
        {
            IsQualifiedWithin3Years = isQualified,
        };
        HttpContext.Session.Set(CreateAccountSessionKey, journeyModel);

        // Act
        var label = Sut.GetIsQualifiedWithin3YearsLabel();

        // Assert
        label.Should().Be(expectedLabel);

        VerifyAllNoOtherCall();
    }
}
