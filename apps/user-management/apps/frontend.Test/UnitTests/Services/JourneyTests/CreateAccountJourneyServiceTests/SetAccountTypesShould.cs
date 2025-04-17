using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.CreateAccountJourneyServiceTests;

public class SetAccountTypesShould : CreateAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_WithExistingSessionData_SetsAccountDetails()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var existingTypes = AccountBuilder.Build().Types;
        var expected = account.Types;
        HttpContext.Session.Set(
            CreateAccountSessionKey,
            new CreateAccountJourneyModel { AccountTypes = existingTypes }
        );

        // Act
        Sut.SetAccountTypes(expected!);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.AccountTypes.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }

    [Fact]
    public void WhenCalled_WithBlankSession_SetsAccountDetails()
    {
        // Arrange
        var account = AccountBuilder.Build();
        var expected = account.Types;

        // Act
        Sut.SetAccountTypes(expected!);

        // Assert
        HttpContext.Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );

        createAccountJourneyModel.Should().NotBeNull();
        createAccountJourneyModel!.AccountTypes.Should().BeEquivalentTo(expected);

        VerifyAllNoOtherCall();
    }
}
