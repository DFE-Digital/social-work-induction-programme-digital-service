using System.Collections.Immutable;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Models;

public class EditAccountJourneyModelTests
{
    [Theory]
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
        var account = new AccountBuilder()
            .WithSocialWorkEnglandNumber(sweId)
            .WithStatus(currentStatus)
            .WithTypes(ImmutableList.Create(AccountType.EarlyCareerSocialWorker))
            .Build();

        var editModel = new EditAccountJourneyModel(account);

        // Act
        var mappedAccount = editModel.ToAccount();

        // Assert
        mappedAccount.Status.Should().Be(expectedStatus);
    }
}
