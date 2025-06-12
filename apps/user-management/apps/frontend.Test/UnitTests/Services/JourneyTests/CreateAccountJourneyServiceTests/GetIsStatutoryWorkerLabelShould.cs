using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsStatutoryWorkerLabelShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true,  "Yes")]
    [InlineData(false, null)]
    [InlineData(null,  null)]
    public void GetIsStatutoryWorkerLabelLabel_ReturnsYesOnlyWhenTrue(bool? isStatutoryWorker,
        string? expectedLabel)
    {
        // Arrange
        var journeyModel = new CreateAccountJourneyModel
        {
            IsStatutoryWorker = isStatutoryWorker,
        };
        HttpContext.Session.Set(CreateAccountSessionKey, journeyModel);

        // Act
        var label = Sut.GetIsStatutoryWorkerLabel();

        // Assert
        label.Should().Be(expectedLabel);

        VerifyAllNoOtherCall();
    }
}
