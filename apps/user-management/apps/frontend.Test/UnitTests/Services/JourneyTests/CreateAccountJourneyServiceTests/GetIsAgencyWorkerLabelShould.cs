using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsAgencyWorkerLabelShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true,  "Yes")]
    [InlineData(false, "No")]
    [InlineData(null,  null)]
    public void GetIsAgencyWorkerLabelLabel_ReturnsExpectedLabel(bool? isAgencyWorker,
        string? expectedLabel)
    {
        // Arrange
        var journeyModel = new CreateAccountJourneyModel
        {
            IsAgencyWorker = isAgencyWorker,
        };
        HttpContext.Session.Set(CreateAccountSessionKey, journeyModel);

        // Act
        var label = Sut.GetIsAgencyWorkerLabel();

        // Assert
        label.Should().Be(expectedLabel);

        VerifyAllNoOtherCall();
    }
}
