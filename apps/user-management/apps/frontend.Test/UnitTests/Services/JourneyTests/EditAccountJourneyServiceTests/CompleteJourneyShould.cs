using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Services.JourneyTests.EditAccountJourneyServiceTests;

public class CompleteJourneyShould : EditAccountJourneyServiceTestBase
{
    [Fact]
    public void WhenCalled_CompletesJourney()
    {
        // Arrange
        var account = AccountFaker.Generate();

        MockAccountRepository.Setup(x => x.GetById(account.Id)).Returns(account);
        MockAccountRepository.Setup(x => x.Update(account));

        Sut.SetIsStaff(account.Id, account.IsStaff);

        // Act
        Sut.CompleteJourney(account.Id);

        // Assert
        HttpContext.Session.TryGet(
            EditAccountSessionKey(account.Id),
            out EditAccountJourneyModel? editAccountJourneyModel
        );

        editAccountJourneyModel.Should().BeNull();

        MockAccountRepository.Verify(x => x.GetById(account.Id), Times.Exactly(2));
        MockAccountRepository.Verify(
            x => x.Update(MoqHelpers.ShouldBeEquivalentTo(account)),
            Times.Once
        );
        VerifyAllNoOtherCall();
    }
}
