using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class GetIsRegisteredWithSocialWorkEnglandLabelShould : CreateAccountJourneyServiceTestBase
{
    [Theory]
    [InlineData(true,  "Yes")]
    [InlineData(false, null)]
    [InlineData(null,  null)]
    public void GetIsRegisteredWithSocialWorkEnglandLabel_ReturnsYesOnlyWhenTrue(bool? isRegistered,
        string? expectedLabel)
    {
        // Arrange
        var journeyModel = new CreateAccountJourneyModel
        {
            IsRegisteredWithSocialWorkEngland = isRegistered
        };
        HttpContext.Session.Set(CreateAccountSessionKey, journeyModel);

        // Act
        var label = Sut.GetIsRegisteredWithSocialWorkEnglandLabel();

        // Assert
        label.Should().Be(expectedLabel);

        VerifyAllNoOtherCall();
    }
}
