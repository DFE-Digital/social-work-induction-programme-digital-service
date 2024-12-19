using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Models;

public class EditAccountJourneyModelTests
{
    [Theory]
    [InlineData(AccountStatus.Paused, AccountStatus.Paused, null)]
    [InlineData(AccountStatus.Inactive, AccountStatus.Inactive, null)]
    [InlineData(AccountStatus.PendingRegistration, AccountStatus.PendingRegistration, null)]
    [InlineData(AccountStatus.Active, AccountStatus.Active, "123")]
    [InlineData(AccountStatus.PendingRegistration, AccountStatus.Active, "123")]
    [InlineData(AccountStatus.Active, AccountStatus.PendingRegistration, null)]
    public void WhenMapped_AccountStatusIsCorrect(
        AccountStatus currentStatus,
        AccountStatus expectedStatus,
        string? sweId
    )
    {
        // Arrange
        var account = new AccountFaker().GenerateSocialWorkerWithSweNumber(sweId, currentStatus);
        var editModel = new EditAccountJourneyModel(account);

        // Act
        var mappedAccount = editModel.ToAccount();

        // Assert
        mappedAccount.Status.Should().Be(expectedStatus);
    }
}
