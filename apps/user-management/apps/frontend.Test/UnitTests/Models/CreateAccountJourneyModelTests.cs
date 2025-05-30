using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Models;

public class CreateAccountJourneyModelTests
{
    [Theory]
    [InlineData(new[] {AccountType.EarlyCareerSocialWorker}, AccountStatus.PendingRegistration)]
    [InlineData(new [] {AccountType.Assessor}, AccountStatus.Active)]
    [InlineData(new[] {AccountType.Coordinator}, AccountStatus.Active)]
    [InlineData(new[] {AccountType.Assessor, AccountType.Coordinator}, AccountStatus.Active)]
    public void WhenMapped_AccountStatusIsCorrect(
        AccountType[] accountTypes,
        AccountStatus expectedStatus
    )
    {
        // Arrange
        var createModel = new CreateAccountJourneyModel
        {
            AccountTypes = accountTypes.ToImmutableList()
        };

        // Act
        var mappedAccount = createModel.ToAccount();

        // Assert
        mappedAccount.Status.Should().Be(expectedStatus);
    }
}
