using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models.NameMatch;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.SocialWorkEnglandServiceTests;

public class GetNameMatchScoreShould : SocialWorkEnglandTestBase
{
    [Fact]
    public void WhenCalled_ReturnNameMatchScore()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Smith";
        var socialWorker = SocialWorkerFaker.GenerateWithName(firstName, lastName);
        var expectedResponse = MatchResult.Exact;

        MockSocialWorkerValidatorService
            .Setup(x => x.ConvertToResult(firstName, lastName, socialWorker.RegisteredName!))
            .Returns(expectedResponse);

        // Act
        var response = Sut.GetNameMatchScore(firstName, lastName, socialWorker.RegisteredName);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(expectedResponse);

        MockSocialWorkerValidatorService.Verify(
            x => x.ConvertToResult(firstName, lastName, socialWorker.RegisteredName!),
            Times.Once
        );
        MockSocialWorkerValidatorService.VerifyNoOtherCalls();

        MockClient.VerifyNoOtherCalls();
    }

    [Fact]
    public void WhenCalledWithNullParameters_ReturnNull()
    {
        // Arrange

        // Act
        var response = Sut.GetNameMatchScore(null, null, null);

        // Assert
        response.Should().BeNull();

        MockSocialWorkerValidatorService.VerifyNoOtherCalls();
        MockClient.VerifyNoOtherCalls();
    }
}
