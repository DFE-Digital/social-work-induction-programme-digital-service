using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Models;

public class CreateUserJourneyModelTests
{
    [Theory]
    [InlineData(new[] {UserType.EarlyCareerSocialWorker}, UserStatus.PendingRegistration)]
    [InlineData(new [] {UserType.Assessor}, UserStatus.Active)]
    [InlineData(new[] {UserType.Coordinator}, UserStatus.Active)]
    [InlineData(new[] {UserType.Assessor, UserType.Coordinator}, UserStatus.Active)]
    public void WhenMapped_AccountStatusIsCorrect(
        UserType[] userTypes,
        UserStatus expectedStatus
    )
    {
        // Arrange
        var createModel = new CreateUserJourneyModel
        {
            UserTypes = userTypes.ToImmutableList()
        };

        // Act
        var mappedAccount = createModel.ToUser();

        // Assert
        mappedAccount.Status.Should().Be(expectedStatus);
    }
}
